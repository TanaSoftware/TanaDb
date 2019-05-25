var messageHandler = {

    Show: function (msg) {

        var htmlName = 'Message.html';

        $.get(htmlName, function (html) {

            $(html).appendTo('body').modal();

            $('#msgText').html(msg);

        });



    },

    close: function () {

        $("#msgHandlerDiv").modal('hide');

        $('.modal-backdrop,fade.show').remove();

        $("#msgHandlerDiv").remove();

    }

}



var selectizeService = {

    InitSelectize: function (ID, Value, Title, Height, Options, SetValue) {



        /// selectize object initiation

        var selectizeMe = $("#" + ID)

            .selectize({ valueField: Value, labelField: Title, searchField: Title, options: Options });



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

        selectize.addOption({ value: val, text: Title });

        selectize.refreshOptions();

    }

}



function showTooltip(txtId, text) {



    var obj = $('#' + txtId);

    obj.tooltip('hide');

    obj.tooltip({

        title: text

    });

    obj.tooltip('show');

    obj.focus();



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