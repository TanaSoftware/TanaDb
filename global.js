var messageHandler = {

    Confirm: function (msg, Ok, Cancel) {
        var v = sessionStorage.getItem("version");
        var htmlName = 'Confirm.html?v=' + v;

        $.get(htmlName, function (html) {

            var obj = document.getElementById("dvConfirmMsg");

            if (obj == null)
                $(html).appendTo('body').modal();


            $("#btnModalContinue").unbind().click(function () {

            });
            $("#btnModalClose").unbind().click(function () {

            });
            $("#btnModalContinue").click(Ok);
            $("#btnModalClose").click(Cancel);

            $('#dvConfirmMsg').html(msg);
            $('#dvConfirmModal').modal('show');

        });
    },

    Show: function (msg) {
        var v = sessionStorage.getItem("version");
        var htmlName = 'Message.html?v=' + v;

        $.get(htmlName, function (html) {
            var obj = document.getElementById("msgHandlerDiv");

            if (obj == null)
                $(html).appendTo('body').modal();

            $('#msgHandlerDiv').modal('show');
            $('#msgText').html(msg);

        });



    },

    close: function () {

        $("#msgHandlerDiv").modal('hide');

        $('.modal-backdrop,fade.show').remove();

    }

}



var selectizeService = {

    setValue: function (id, val) {
        var selectizeMe = $("#" + id);
        var selectize = selectizeMe[0].selectize;
        selectize.setValue(val);
    },
    InitSelectize: function (ID, Value, Title, Height, Options, SetValue) {



        /// selectize object initiation

        var selectizeMe = $("#" + ID)

            .selectize({ valueField: Value, labelField: Title, searchField: Title, options: Options });


        if (selectizeMe[0] == null)
            return;
        /// selectize object

        var selectize = selectizeMe[0].selectize;



        /// case there is a default value

        if (SetValue) {



            /// set default

            selectize.setValue(SetValue);

        }



        /// selectize control

        var control = selectize.$control[0];



        if (Height) {

            control.style["height"] = Height + "px";

            control.style["padding"] = "6px 12px";

        }



    },

    AddOption: function (ID, val, Title) {

        var selectizeMe = $("#" + ID)

        var selectize = selectizeMe[0].selectize;

        selectize.addItem({ value: val, text: Title });

        //selectize.refreshOptions();

    },
    ClearAll: function (Id) {
        $('#' + Id).selectize()[0].selectize.destroy();
    }
}

function SetHour() {
    try {
        document.querySelectorAll('.input-time').forEach(function (el) {
            new Cleave(el,
                {
                    time: true,
                    timePattern: ["h", "m"]
                });
        });
    }
    catch (e) { }
}

function showTooltip(txtId, text) {

    var obj = $('#' + txtId);

    obj.tooltip('hide');

    obj.tooltip({

        title: text

    });

    obj.tooltip('show');

    obj.focus();
    event.stopPropagation();
    event.preventDefault();
}

function hideTooltip(txtId) {

    var obj = $('#' + txtId);

    obj.tooltip('hide');

}



function isHour(htStr) {

    var IsColon = true;

    var hrCh = ":";

    var fstr = htStr

    if (fstr.length < 1) {

        return true;

    }



    var pos1 = htStr.indexOf(hrCh)

    if (pos1 < 0) {

        IsColon = false;

        pos1 = 2;

        if (htStr.length > 4)

            return false;

        if (htStr.length == 3)

            pos1 = 1;

    }

    var iLen = 1;

    if (!IsColon)

        iLen = 0;

    var strHour = htStr.substring(0, pos1)

    var strMinute = htStr.substring(pos1 + iLen)



    if (strHour.charAt(0) == "0" && strHour.length > 1)

        strHour = strHour.substring(1);

    if (strMinute.charAt(0) == "0" && strMinute.length > 1)

        strMinute = strMinute.substring(1);



    var hour = parseInt(strHour)

    var minute = parseInt(strMinute)

    if (isNaN(hour))

        return false;



    if (isNaN(minute))

        return false;



    if (strHour.length < 1 || hour < 0 || hour > 23) {

        return false

    }

    if (strMinute.length < 1 || minute < 0 || minute > 59) {

        return false

    }



    return true

}

function IsvalidEmail(mail) {

    var mArr = mail.split('@');

    if (mArr.length <= 1) {

        return false;

    }

    var arrPoint = mArr[1].split('.');

    if (arrPoint.length <= 1) {

        return false;

    }



    return true;

}

function IsvalidPhone(num) {

    if (num.length < 10 || num.length > 10) {

        return false;

    }

    return true;

}



function getCities(handleData) {



    $.ajax({

        url: "User/GetCity",

        type: 'GET',

        contentType: "application/json;charset=utf-8",

        success: function (data) {

            if (data != null)

                handleData(data);



        },

        error: function (request, status, error) {



        }

    });

}


var CookieManager = {
    //Create Cookie
    days: 1,
    CreateCookie: function (name, value, day) {
        //Create New Cookie
        //var expires = "";
        if (day != null)
            this.days = day;
        var date = new Date();
        date.setTime(date.getTime() + (this.days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
        document.cookie = name + "=" + value + expires + "; path=/";
    },
    //Read Cookie
    ReadCookie: function (name) {
        //Get Cookie value
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    DeleteCookie: function (name, value) {
        this.CreateCookie(name, "", -1);
    }
}

function goToCal(data) {


    if (data != null) {

        sessionStorage.setItem("UserData", JSON.stringify(data));
        var v = sessionStorage.getItem("version");
        location.href = "cal.html?v=" + v;

    }

}
function goToStart() {

    var v = sessionStorage.getItem("version");
    location.href = "Start.html?v=" + v;

}

function Login() {
    var userObj = { Mail: "", Password: "" };

    if (localStorage.chkbx && localStorage.chkbx != '') {
        userObj.Mail = localStorage.usrname.toLowerCase();

        userObj.Password = localStorage.pass;
    }
    else {


        var mail = $('#emailLogin').val();
        var pass = $('#passwordLogin').val();

        if (!checkValid("emailLogin", "נא להזין מייל")) {

            return;

        }

        if (!checkValid("passwordLogin", "נא להזין סיסמה")) {

            return;

        }


        userObj.Mail = mail.toLowerCase();

        userObj.Password = pass;
    }


    $.ajax({

        url: "User/Login",

        type: 'POST',

        data: JSON.stringify(userObj),

        contentType: "application/json;charset=utf-8",

        success: function (data) {

            if (data != null) {

                if ($('#remember_me').is(':checked')) {
                    localStorage.usrname = $('#emailLogin').val().toLowerCase();
                    localStorage.pass = $('#passwordLogin').val();
                    localStorage.chkbx = $('#remember_me').val();

                }

                goToCal(data);

            }

            else {

                messageHandler.Show("משתמש לא קיים");

            }

        },

        error: function (request, status, error) {

        }

    });

}

function checkValid(txtId, textMsg) {

    $('#' + txtId).tooltip('hide');

    var txt = $("#" + txtId).val();

    if (txt.length <= 0) {

        $('#' + txtId).tooltip({

            title: textMsg

        });

        $('#' + txtId).tooltip('show');

        $('#' + txtId).focus();

        return false;

    }

    return true;

}

function checkHour(txtId, textMsg) {

    $('#' + txtId).tooltip('hide');

    if (!isHour($("#" + txtId).val())) {

        $('#' + txtId).tooltip({

            title: textMsg

        });

        $('#' + txtId).tooltip('show');

        $('#' + txtId).focus();

        return false;

    }

    return true;

}

function goBack() {
    history.back();
}

function checkValidDDl(txtId, textMsg) {

    $('#' + txtId).tooltip('hide');

    var txt = $("#" + txtId).val();

    if (txt == "0" || txt == "") {

        //showTooltip(txtId, textMsg);

        $('#' + txtId).tooltip({

            title: textMsg

        });

        $('#' + txtId).tooltip('show');

        $('#' + txtId).focus();

        return false;

    }

    return true;

}

function testPassword(pwString) {
    var strength = 0;
    var strengthLang = 0;
    strengthLang += /[א-ת]+/.test(pwString) ? 1 : 0;
    strengthLang += /[A-Z]+/.test(pwString) ? 1 : 0;
    strengthLang += /[a-z]+/.test(pwString) ? 1 : 0;
    strength += /[0-9]+/.test(pwString) ? 1 : 0;


    if (strengthLang + strength < 2)
        return false;

    return true;
}

function SetHourToDddl() {
    document.querySelectorAll('.input-time').forEach(function (el) {
        new Cleave(el,
            {
                time: true,
                timePattern: ["h", "m"]
            });
    });

}

function isValidDate(dtStr) {
    var dtCh = "/";
    var minYear = 1900;
    var maxYear = 2100;
    var fstr = dtStr;
    if (fstr.length < 1) {
        return true;
    }
    var daysInMonth = [0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    var pos1 = dtStr.indexOf(dtCh);
    var pos2 = dtStr.indexOf(dtCh, pos1 + 1);
    var strDay = dtStr.substring(0, pos1);
    var strMonth = dtStr.substring(pos1 + 1, pos2);
    var strYear = dtStr.substring(pos2 + 1);
    var strYr = strYear;
    if (strDay.charAt(0) == "0" && strDay.length > 1) { strDay = strDay.substring(1); }
    if (strMonth.charAt(0) == "0" && strMonth.length > 1) { strMonth = strMonth.substring(1); }
    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) { strYr = strYr.substring(1); }
    }
    var month = parseInt(strMonth);
    if (isNaN(month))
        return false;
    var day = parseInt(strDay);
    if (isNaN(day))
        return false;
    var year = parseInt(strYr);
    if (isNaN(year))
        return false;

    if (strMonth.length < 1 || month < 1 || month > 12) {
        return false;
    }
    if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > this.daysInFebruary(year)) || day > daysInMonth[month]) {
        return false;
    }
    if (strYear.length != 4 || year == 0 || year < minYear || year > maxYear) {
        return false;
    }

    return true;
}

function goToUrl(url) {
    var v = sessionStorage.getItem("version");
    location.href = url + '?v=' + v;
}



function forgetPassword() {
    var mail = $('#emailLogin').val();

    if (mail.length <= 0) {

        showTooltip("emailLogin", "נא להזין מייל");

        return;

    }

    if (!IsvalidEmail(mail)) {

        showTooltip("emailLogin", "נא להזין מייל תקין");

        return;

    }

    mail = mail.replace(".", '!!');
    mail = mail.replace(".", '!!');


    $.ajax({

        url: "User/ResetPasswordPhase1/" + mail,

        type: 'GET',

        contentType: "application/json;charset=utf-8",

        success: function (data) {
            if (data != "") {
                messageHandler.Show(data);
                return;
            }
            messageHandler.Show("נשלח אליך מייל לאיפוס הסיסמה");

        },

        error: function (request, status, error) {



        }

    });
}

function replaceAll(target, search, replacement) {

    return target.split(search).join(replacement);

}

function IsValidText(inputtxt) {
    var letters = /^[א-תA-Za-z '-]+$/;
    if (inputtxt.match(letters)) {

        return true;
    }
    else {
        return false;
    }
}
function IsValidEngText(inputtxt) {
    var letters = /^[A-Za-z]+$/;
    if (inputtxt.match(letters)) {

        return true;
    }
    else {
        return false;
    }
}

function IsValidTextAndNumbers(inputtxt) {
    var letters = /^[א-תA-Za-z0-9 -']+$/;
    if (inputtxt.match(letters)) {

        return true;
    }
    else {
        return false;
    }
}

function IsValidAddress(inputtxt) {
    var letters = /^[א-תA-Za-z0-9 /\-']+$/;
    if (inputtxt.match(letters)) {

        return true;
    }
    else {
        return false;
    }
}