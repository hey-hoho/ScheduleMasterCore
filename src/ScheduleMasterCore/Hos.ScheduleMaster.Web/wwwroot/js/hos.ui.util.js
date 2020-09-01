
(function ($) {
    var hos = window.hos = window.hos || {};

    var Util = function () {

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
                        if (xhrResult.Message != null && xhrResult.Message.length > 0) {
                            messager(xhrResult.Message, 'success', xhrResult.Url);
                        }
                        if (typeof (this.callback) === "function") {
                            this.callback(xhrResult);
                        }
                    } else {
                        messager(xhrResult.Message, 'warning');
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
            $('#mySmModal').modal('show')
                .on('shown.bs.modal', function () {
                    $('#mySmModal-func button:last').focus();
                })
                .on('hidden.bs.modal', function () {
                    $('#mySmModal').remove();
                });
        }
    }

    Util.prototype.alert = function (message) {
        var html = "<i class=\"fa fa-volume-up\" style=\"vertical-align: middle;color:#03a2b6;font-size: 30px;\"></i><div style=\"display: table-cell;vertical-align: middle;margin: 0;padding-left: 1em;\"><strong>" + message + "</strong></div></div>";
        this.baseDialog(html, [], true);
    }

    Util.prototype.dialog = function (message, buttons) {
        var html = "<i class=\"fa fa-info-circle\" style=\"vertical-align: middle;color:#03a2b6;font-size: 30px;\"></i><div style=\"display: table-cell;vertical-align: middle;margin: 0;padding-left: 1em;\"><strong>" + message + "</strong></div></div>";
        this.baseDialog(html, buttons, false);
    }

    Util.prototype.confirm = function (message, callback) {
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
    Util.prototype.successTip = function (text, redirect) {
        this.messager(text, 'success', redirect);
    };

    //错误提示
    Util.prototype.errorTip = function (text, redirect) {
        this.messager(text, 'error', redirect);
    };

    //信息提示
    Util.prototype.infoTip = function (text, redirect) {
        this.messager(text, 'info', redirect);
    };

    //警告提示
    Util.prototype.warningTip = function (text, redirect) {
        this.messager(text, 'warning', redirect);
    };

    //异步get请求
    Util.prototype.getJson = function (url, data, callback) {
        this.ajax(url, "GET", "json", data, callback);
    };

    //异步post请求
    Util.prototype.postJson = function (url, data, callback) {
        this.ajax(url, "POST", "json", data, callback);
    };

    //表单验证
    Util.prototype.formValidate = function (form, rule, message) {
        var validator = $("#" + form).validate({
            onfocusout: false,
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
        return validator;
    };

    //提交表单
    Util.prototype.formSubmit = function (form, requset) {
        var $form = $("#" + form);
        $("#sbtn_" + form).bind("click", function (e) {
            if (!$form.valid()) { return false; }
            var data = $form.getFormData();
            requset(data);
            e.preventDefault();
        });
    }

    //生成guid
    Util.prototype.guid = function () {
        var id = "", i, random;
        for (i = 0; i < 32; i++) {
            random = Math.random() * 16 | 0;
            if (i == 8 || i == 12 || i == 16 || i == 20) {
                id += "-";
            }
            id += (i == 12 ? 4 : (i == 16 ? (random & 3 | 8) : random)).toString(16);
        }
        return id;
    };

    //时间格式化
    Util.prototype.getFormatDate = function (date, dateformat) {
        dateformat = dateformat || "yyyy/MM/dd hh:mm:ss";
        if (typeof date === 'string') {
            date = new Date(Date.parse(date));
        }
        if (!date || isNaN(date)) return null;
        var format = dateformat;
        var o = {
            "M+": date.getMonth() + 1,
            "d+": date.getDate(),
            "h+": date.getHours(),
            "m+": date.getMinutes(),
            "s+": date.getSeconds(),
            "q+": Math.floor((date.getMonth() + 3) / 3),
            "S": date.getMilliseconds()
        }
        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (date.getFullYear() + "")
                .substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k]
                    : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    };

    //复制文本到剪贴板
    Util.prototype.copyToClip = function (content) {
        var aux = document.createElement("input");
        aux.setAttribute("value", content);
        document.body.appendChild(aux);
        aux.select();
        document.execCommand("copy");
        document.body.removeChild(aux);
        this.messager("复制成功", "success");
    }

    //获取cookie项
    Util.prototype.getCookiesItem = function (sKey) {
        if (!sKey) { return null; }
        return decodeURIComponent(document.cookie.replace(new RegExp("(?:(?:^|.*;)\\s*" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*([^;]*).*$)|^.*$"), "$1")) || null;
    };

    //设置cookie项
    Util.prototype.setCookiesItem = function (sKey, sValue, vEnd, sPath, sDomain, bSecure) {
        if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) { return false; }
        var sExpires = "";
        if (vEnd) {
            switch (vEnd.constructor) {
                case Number:
                    sExpires = vEnd === Infinity ? "; expires=Fri, 31 Dec 9999 23:59:59 GMT" : "; max-age=" + vEnd;
                    break;
                case String:
                    sExpires = "; expires=" + vEnd;
                    break;
                case Date:
                    sExpires = "; expires=" + vEnd.toUTCString();
                    break;
            }
        }
        document.cookie = encodeURIComponent(sKey) + "=" + encodeURIComponent(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
        return true;
    };

    //删除cookie项
    Util.prototype.removeCookiesItem = function (sKey, sPath, sDomain) {
        if (!this.hasItem(sKey)) { return false; }
        document.cookie = encodeURIComponent(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "");
        return true;
    };

    //是否存在cookie项
    Util.prototype.hasCookiesItem = function (sKey) {
        if (!sKey) { return false; }
        return (new RegExp("(?:^|;\\s*)" + encodeURIComponent(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
    };

    Util.prototype.getCookiesKeys = function () {
        var aKeys = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
        for (var nLen = aKeys.length, nIdx = 0; nIdx < nLen; nIdx++) { aKeys[nIdx] = decodeURIComponent(aKeys[nIdx]); }
        return aKeys;
    }

    Util.prototype.breadCrumb = function (value) {
        window.parent.renderBreadcrumb(value);
    }

    hos.ui = hos.ui || {};
    $.extend(hos.ui, {
        util: new Util()
    });
})(jQuery);


