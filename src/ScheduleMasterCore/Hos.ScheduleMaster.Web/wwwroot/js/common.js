

var Common = function () {

    //漂浮消息的基类
    var messager = function (text, type, redirect) {
        var timeout = ((text.length < 13 ? 1 : text.length / 13) * 1300).toString();
        var position = 'top-center';//top-right
        switch (type) {
            case 'success': Notify(text, position, timeout, 'success', '', true); break;
            case 'warning': Notify(text, position, timeout, 'warning', '', true); break;
            case 'error': Notify(text, position, timeout, 'error', '', true); break;
            case 'info': 
            default: Notify(text, position, timeout, 'info', '', true); break;
        }
        if (typeof (redirect) === "string" && redirect.length > 0) {
            setTimeout(function () { location.href = redirect; }, timeout);
        }
    }
    this.messager = messager;

    //通用异步方法
    this.ajaxOption = function (url, type) {
        return {
            url: url,
            type: type == undefined ? "post" : type,
            dataType: "json",
            timeout: 6000,
            beforeSend: function () {
                //showLoading();
            },
            complete: function () {
                //hideLoading();
            },
            success: function (xhrResult) {
                if (xhrResult.Success === true) {
                    if (xhrResult.Msg != null && xhrResult.Msg.length > 0) {
                        messager(xhrResult.Msg, 'success', xhrResult.Url);
                    }
                    if (typeof (this.callback) === "function") {
                        this.callback(xhrResult);
                    }
                } else {
                    messager(xhrResult.Msg, 'warning');
                }
            },
            error: function (xhr, errorType, error) {
                messager('Ajax request error : ' + error, 'error');
            }
        }
    }

    this.ajax = function (url, type, dataType, data, callback) {
        var option = this.ajaxOption(url, type);
        option.dataType = dataType;
        option.data = data;
        option.callback = callback;
        $.ajax(option);
    }

    this.baseDialog = function (message, buttons, close) {
        var html =
            "<div class=\"modal fade\" data-backdrop=\"static\" id=\"mySmModal\"><div class=\"modal-dialog modal-moveable modal-dragged modal-sm\"><div class=\"modal-content\"><div class=\"modal-body\"><div style=\"display: table;padding:3px 10px\">" + message + "</div><div style=\"padding: 5px;text-align: right;\" id=\"mySmModal-func\" class=\"modal-footer\">";
        if (close === true) {
            html += "<button type=\"button\" class=\"btn btn-sm btn-default\" data-dismiss=\"modal\">关闭</button> ";
        }
        $("body").append(html);
        $.each(buttons, function (index, item) {
            var btn = $("<button type=\"button\" class=\"btn btn-sm " + item.style + "\"  data-dismiss=\"modal\">" + item.text + "</button>");
            btn[0].onclick = item.handler;
            btn.appendTo("#mySmModal-func");
        })
        $("body").append("</div></div></div></div></div>");
        $('#mySmModal').modal('show').on('hidden.bs.modal', function () {
            $('#mySmModal').remove();
        });
    }
}

Common.prototype.alert = function (message) {
    var html = "<i class=\"fa fa-volume-up\" style=\"vertical-align: middle;color:#03a2b6;font-size: 30px;\"></i><div style=\"display: table-cell;vertical-align: middle;margin: 0;padding-left: 1em;\"><strong>" + message + "</strong></div></div>";
    this.baseDialog(html, [], true);
}

Common.prototype.dialog = function (message, buttons) {
    var html = "<i class=\"fa fa-info-circle\" style=\"vertical-align: middle;color:#03a2b6;font-size: 30px;\"></i><div style=\"display: table-cell;vertical-align: middle;margin: 0;padding-left: 1em;\"><strong>" + message + "</strong></div></div>";
    this.baseDialog(html, buttons, false);
}

Common.prototype.confirm = function (message, callback) {
    var html = "<i class=\"fa fa-question-circle\" style=\"vertical-align: middle;color:#f08c78;font-size: 30px;\"></i><div style=\"display: table-cell;vertical-align: middle;margin: 0;padding-left: 1em;\"><strong>" + message + "</strong></div></div>";
    var button = [];
    button[0] = {
        text: '确定',
        style: 'btn-primary',
        handler: callback
    };
    this.baseDialog(html, button, true);
}

//成功提示
Common.prototype.successTip = function (text, redirect) {
    this.messager(text, 'success', redirect);
};

//错误提示
Common.prototype.errorTip = function (text, redirect) {
    this.messager(text, 'error', redirect);
};

//信息提示
Common.prototype.infoTip = function (text, redirect) {
    this.messager(text, 'info', redirect);
};

//警告提示
Common.prototype.warningTip = function (text, redirect) {
    this.messager(text, 'warning', redirect);
};

//异步get请求
Common.prototype.getJson = function (url, data, callback) {
    this.ajax(url, "GET", "json", data, callback);
};

//异步post请求
Common.prototype.postJson = function (url, data, callback) {
    this.ajax(url, "POST", "json", data, callback);
};

//表单验证
Common.prototype.formValidate = function (form, rule, message) {
    $("#" + form).validate({
            rules: rule,
            messages: message,
            errorClass: "form-control-error",
            validClass: "has-success",
            ignore: ".ignore",
            errorElement: "small",
            highlight: function (element, errorClass, validClass) {
                $(element).parents(".form-group").removeClass("has-success").addClass("has-feedback has-error");
                $(element).parent().find("i").remove();
                $(element).after('<i class="form-control-feedback glyphicon glyphicon-remove"></i>');
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).parents(".form-group").removeClass("has-error").addClass("has-feedback  has-success");
                $(element).parent().find("i").remove();
                $(element).after('<i class="form-control-feedback glyphicon glyphicon-ok"></i>');
            },
            errorPlacement: function (error, element) {
                var parent = $(element).parent();
                if (parent.hasClass("input-group")) {
                    error.appendTo(parent.parent());
                } else {
                    error.appendTo(parent);
                }
            }
        });
};

//通用工具类实例
var $tools = new Common();