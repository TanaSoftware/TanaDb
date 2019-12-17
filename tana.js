String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};


var cScriptLoader = (function () {
    function cScriptLoader(files) {
        var _this = this;
        this.log = function (t) {
            //console.log("ScriptLoader: " + t);
        };
        this.withNoCache = function (filename) {
            //if (filename.indexOf("?") === -1)
            //    filename += "?no_cache=" + new Date().getTime();
            //else
            //    filename += "&no_cache=" + new Date().getTime();
            return filename;
        };
        this.loadStyle = function (filename) {
            // HTMLLinkElement
            var link = document.createElement("link");
            link.rel = "stylesheet";
            link.type = "text/css";
            link.href = _this.withNoCache(filename);
            _this.log('Loading style ' + filename);
            link.onload = function () {
                _this.log('Loaded style "' + filename + '".');
            };
            link.onerror = function () {
                _this.log('Error loading style "' + filename + '".');
            };
            _this.m_head.appendChild(link);
        };
        this.loadScript = function (i) {
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = _this.withNoCache(_this.m_js_files[i]);
            var loadNextScript = function () {
                if (i + 1 < _this.m_js_files.length) {
                    _this.loadScript(i + 1);
                }
            };
            script.onload = function () {
                _this.log('Loaded script "' + _this.m_js_files[i] + '".');
                loadNextScript();
            };
            script.onerror = function () {
                _this.log('Error loading script "' + _this.m_js_files[i] + '".');
                loadNextScript();
            };
            _this.log('Loading script "' + _this.m_js_files[i] + '".');
            _this.m_head.appendChild(script);
        };
        this.loadFiles = function () {
            // this.log(this.m_css_files);
            // this.log(this.m_js_files);
            for (var i = 0; i < _this.m_css_files.length; ++i)
                _this.loadStyle(_this.m_css_files[i]);
            _this.loadScript(0);
        };
        this.m_js_files = [];
        this.m_css_files = [];
        this.m_head = document.getElementsByTagName("head")[0];
        // this.m_head = document.head; // IE9+ only
        function endsWith(str, suffix) {
            if (str === null || suffix === null)
                return false;
            return str.indexOf(suffix, str.length - suffix.length) !== -1;
        }
        for (var i = 0; i < files.length; ++i) {
            if (endsWith(files[i], ".css")) {
                this.m_css_files.push(files[i]);
            }
            else if (endsWith(files[i], ".js")) {
                this.m_js_files.push(files[i]);
            }
            else
                this.log('Error unknown filetype "' + files[i] + '".');
        }
    }
    return cScriptLoader;
})();


init();

function init() {

    GetSources();
}
function SetSource(id, source,params) {
    var elem = document.getElementById(id);
    elem.setAttribute("source", source);
    if (elem.firstChild == null)
        return;
    var InHtml = elem.firstChild.innerHTML;
    if (InHtml==null)
        InHtml = elem.innerHTML
    if (params != null) {
        if (typeof params =="string" && typeof window[params] == "object")
            params = window[params];

        PostRest(source, params, function successGet(data) {
            if (data == null || data.length == 0) {
                elem.innerHTML = "<div style='display:none'>" + InHtml + "</div>";
                return;
            }
            var htm = getDigestHtml(data, InHtml);

            if (htm != "") {
                elem.innerHTML = SetDivWrapper(InHtml, htm);
                elem.style.display = "block";
            }
            else
                elem.innerHTML = "<div style='display:none'>" + InHtml + "</div>";
        }, errorGet);
    }
    else {
        GetRest(source, function successGet(data) {

            var template = InHtml;
            if (data == null || data.length == 0) {
                elem.innerHTML = "<div style='display:none'>" + InHtml + "</div>";
                return;
            }

            var htm = getDigestHtml(data, InHtml);

            //var templateId = "dvTemplate__" + i;
            if (htm != "") {
                elem.innerHTML = SetDivWrapper(template, htm);
                elem.style.display = "block";
            }
            else
                elem.innerHTML = "<div style='display:none'>" + InHtml + "</div>";
        }, errorGet);
    }
}

function loadScript(path, callback) {

    var done = false;
    var scr = document.createElement('script');

    scr.onload = handleLoad;
    scr.onreadystatechange = handleReadyStateChange;
    scr.onerror = handleError;
    scr.src = path;
    document.body.appendChild(scr);

    function handleLoad() {
        if (!done) {
            done = true;
            callback(path, "ok");
        }
    }

    function handleReadyStateChange() {
        var state;

        if (!done) {
            state = scr.readyState;
            if (state === "complete") {
                handleLoad();
            }
        }
    }
    function handleError() {
        if (!done) {
            done = true;
            callback(path, "error");
        }
    }
}
function render(data, targetId, objName) {
    var obj = document.getElementById(targetId);
    if (obj == null)
        return;
    var template = obj.childNodes[0].tagName  == "DIV" ? obj.childNodes[0].innerHTML : obj.innerHTML;
    var htm = getDigestHtml(data, template, objName);
    obj.innerHTML = SetDivWrapper(template, htm);
    obj.style.display = "block";
}

function getDigestHtml(data, InHtml,objName) {
    var htm = "";
    if (typeof data == "object") {
        if (data.length == undefined) {
            var replaceMe = InHtml;

            for (var r in data) {
                replaceMe = replaceMe.replaceAll("{{" + r + "}}", data[r]);
                //replaceMe = replaceMe.replace("[ModelBind]", "onBlur=alert('" + r + "')");
            }
            htm += replaceMe;
        }
        else {
            for (var j = 0; j < data.length; j++) {
                var rowData = data[j];
                var replaceMe = InHtml;

                for (var r in rowData) {
                    replaceMe = replaceMe.replaceAll("{{" + r + "}}", rowData[r]);
                }
                htm += replaceMe;
            }
        }
    }
    else {
        var replaceMe = InHtml;
        replaceMe = replaceMe.replaceAll("{{" + objName + "}}", data);
        htm += replaceMe;
    }
    return htm;
}

function SetDivWrapper(template, htm) {
    return "<div style='display:none'>" + template + "</div>" + htm;
}

function getclientSources(clientSources, scripts) {
    for (var i = 0; i < clientSources.length; i++) {
        clientSources[i].style.display = "none";
        var template = clientSources[i].innerHTML;
        var module = clientSources[i].getAttribute("module");
        if (module != null) {
            scripts.push(module);
        }
        var obj = clientSources[i].getAttribute("clientSource");        

        
        if (obj != null && obj != "") {
            var htm = "";
            if (obj.indexOf('/') < 0) {
                if (typeof window[obj] == "object") {
                    htm = getDigestHtml(window[obj], template);

                }
                else {
                    htm = getDigestHtml(window[obj], template, obj);

                }
                clientSources[i].innerHTML = SetDivWrapper(template, htm);
                clientSources[i].style.display = "block";
                
            }
            else {
                var arrUrl = obj.split('/');
                var func = arrUrl[2];
                var param = arrUrl.length > 3 ? arrUrl[3] : "";
                loadScript(arrUrl[0] + "/" + arrUrl[1] + ".js", function callBack(data, succes) {
                    if (succes == "ok") {
                        var fn = window[func];
                        if (param.length == 0)
                            fn(template);
                        else
                            fn(template, param);
                    }

                });
            }
        }
    }
}

function getServerallSources(allSources, scripts) {
    for (var i = 0; i < allSources.length; i++) {
        allSources[i].style.display = "none";
        var template = allSources[i].innerHTML;
        var module = allSources[i].getAttribute("module");
        if (module != null) {
            //loadJS(module, onLoadJs, onErrorLoadJs);
            scripts.push(module);
        }
        ///get data source from server
        var url = allSources[i].getAttribute("source");
        var fn = window[url];
        var params = allSources[i].getAttribute("params");

        // is object a function?
        if (typeof fn === "function") {
            fn(template, allSources[i]);
        }
        else {
            if (url != null && url != "") {
                var InHtml = allSources[i].innerHTML;
                (function (i, InHtml, url) {
                    if (params != null && params != "") {//post
                        if (typeof window[params] == "object")
                            params = window[params];

                        PostRest(url, params, function successGet(data) {
                            var htm = getDigestHtml(data, InHtml);

                            allSources[i].style.display = "block";

                            allSources[i].innerHTML = "<div style='display:none'>" + template + "</div>" + htm;
                        }, errorGet);
                    }
                    else {//get
                        GetRest(url, function successGet(data) {
                            var htm = getDigestHtml(data, InHtml);

                            allSources[i].style.display = "block";
                            
                            allSources[i].innerHTML = "<div style='display:none'>" + template + "</div>" + htm;
                        }, errorGet);
                    }
                }).call(this, i, InHtml, url);
            }
        }
    }
}
function GetSources() {
    //var xBody = document.getElementsByTagName("BODY")[0];
    //xBody.style.display = "none";
    var scripts = [];
    var allSources = document.querySelectorAll('[source]');
    var clientSources = document.querySelectorAll('[clientSource]');
    var clientSourcesTempates = document.querySelectorAll('template');

    //clientSourcesTempates
    for (var i = 0; i < clientSourcesTempates.length; i++) {
        var temp = document.querySelector(document.querySelectorAll('template')[i].id).shadowRoot.querySelectorAll("[clientSource]");
        getclientSources(temp, scripts);
        var temp2 = document.querySelector(document.querySelectorAll('template')[i].id).shadowRoot.querySelectorAll("[source]");
        getServerallSources(temp2, scripts);
    }

    //clientSource
    getclientSources(clientSources, scripts);

    //source - server
    getServerallSources(allSources, scripts);

    //xBody.style.display = "block";
    if (scripts != null && scripts.length > 0) {
        var ScriptLoader = new cScriptLoader(scripts);
        ScriptLoader.loadFiles();
    }
}

function onLoadJs(data) {
    var head = document.getElementsByTagName('head');
    var injectedScript = document.createElement('script');
    head[0].appendChild(injectedScript);
    injectedScript.innerHTML = data;
}
function onErrorLoadJs(data) {

}
function errorGet(data,url) {
    
}



function GetRest(url, success, error) {
    var req = new XMLHttpRequest();
    req.onreadystatechange = function () {
        if (this.readyState == 4) {
            
            if (req.status != 200 && req.status != 304) {
                error(this.responseText);
                return;
            }
            if (req.status == 200) {                
                var data = JSON.parse(this.responseText);
                success(data);
            }

        }
    }
    req.open('GET', url, true);
    req.send();

}

function PostRest(url,params, success, error) {
    var req = new XMLHttpRequest();
    
    req.onreadystatechange = function () {
        if (this.readyState == 4) {

            if (req.status != 200 && req.status != 304) {
                error(this.responseText);
                return;
            }
            if (req.status == 200) {
                var data = JSON.parse(this.responseText);
                success(data);
            }

        }
    }
    req.open('POST', url, true);
    req.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    if (typeof params == "string")
        req.send(params);
    else
        req.send(JSON.stringify(params));

}

function loadJS(url, onDone, onError) {
    if (!onDone) onDone = function () { };
    if (!onError) onError = function () { };
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200 || xhr.status == 0) {

                onDone(xhr.responseText);
            } else {
                onError(xhr.status);
            }
        }
    }.bind(this);
    try {
        xhr.open("GET", url, true);
        xhr.send();
    } catch (e) {
        onError(e);
    }
}


