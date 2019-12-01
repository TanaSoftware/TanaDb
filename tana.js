


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
function SetSource(id, source) {
    var elem = document.getElementById(id);
    elem.setAttribute("source", source);
    if (elem.firstChild == null)
        return;
    var InHtml = elem.firstChild.innerHTML;

    GetRest(source, function successGet(data) {
        
        var template = "<div style='display:none'>" + InHtml + "</div>"
        if (data == null || data.length == 0) {
            elem.innerHTML = template;
            return;
        }

        var htm = getDigestHtml(data, InHtml);

        //var templateId = "dvTemplate__" + i;
        if (htm != "")
            elem.innerHTML = template + htm;
    }, errorGet);
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
function build(template,data, targetId) {
    var htm = getDigestHtml(data, template);
    document.getElementById(targetId).innerHTML = htm;
}

function getDigestHtml(data, InHtml) {
    var htm = "";
    for (var j = 0; j < data.length; j++) {
        var rowData = data[j];
        var replaceMe = InHtml;

        for (var r in rowData) {
            replaceMe = replaceMe.replaceAll("{{" + r + "}}", rowData[r]);
        }
        htm += replaceMe;
    }
    return htm;
}

function GetSources() {
    var scripts = [];
    var allSources = document.querySelectorAll('[source]');
    var clientSources = document.querySelectorAll('[clientSource]');

    //clientSource
    for (var i = 0; i < clientSources.length; i++) {
        clientSources[i].style.display = "none";
        var template = clientSources[i].innerHTML;
        var url = clientSources[i].getAttribute("clientSource");
        if (url != null) {
            var arrUrl = url.split('/');
            var func = arrUrl[2];
            var param = arrUrl.length > 3 ? arrUrl[3]:"";
            loadScript(arrUrl[0]+"/"+arrUrl[1]+".js",function callBack(data,succes){
                if (succes=="ok") {
                    var fn = window[func];
                    if(param.length==0)
                        fn(template);
                    else
                        fn(template, param);
                }
                
            });                        
        }
    }

    //source - server
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

        // is object a function?
        if (typeof fn === "function") {
            fn(template, allSources[i]);
        }
        else {
            if (url != null) {
                var InHtml = allSources[i].innerHTML;
                (function (i, InHtml, url) {
                    GetRest(url, function successGet(data) {
                        var htm = getDigestHtml(data,InHtml);
                       
                        allSources[i].style.display = "block";

                        var templateId = "dvTemplate__" + i;
                        allSources[i].innerHTML = "<div id='" + templateId + "' style='display:none'>" + template + "</div>" + htm;
                    }, errorGet);
                }).call(this, i, InHtml, url);
            }
        }
    }
    var ScriptLoader = new cScriptLoader(scripts);
    ScriptLoader.loadFiles();
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

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

function GetRest(url, success, error) {
    var req = new XMLHttpRequest();
    req.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (req.status != 200 && req.status != 304) {
                error(this.responseText);
                return;
            }
            var data = JSON.parse(this.responseText);
            success(data);


        }
    }
    req.open('GET', url, true);
    req.send();

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




