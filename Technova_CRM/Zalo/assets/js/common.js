
/*Define parameters*/
var token = "";
var userID = "";
var userCode = "";
var userFullName = "";
var defaultLoadNumb = 100;

var login_url = '/Website/app/login/';
var localStorageInfo = JSON.parse(storageGetItem("localStorageList"));

var common_api_url = window.location.origin + '/api/';
var common_guid_empty = '00000000-0000-0000-0000-000000000000';


$(document).ready(function () {
    if (localStorageInfo == null) {
        returnLogin();
    } else {
        checkForUpdate();

        try {
            $(".datePickerCustom").datetimepicker({
                format: 'Y-m-d',
                timepicker: false,
            });

            $(".dateTimePickerCustom").datetimepicker({
                format: 'Y-m-d H:m',
                timepicker: false,
            });
        } catch (e) {
            console.log(e);
        }

        try {
            toastr.options.showMethod = "slideDown";
            toastr.options.hideMethod = "slideUp";
            toastr.options.closeMethod = "slideUp";
            toastr.options.progressBar = true;
            toastr.options.rtl = true;
            toastr.options.positionClass = "toast-bottom-right";
        } catch (e) {
            console.log(e);
        }

        //
        token = localStorageInfo.data.token.Key;
        userID = localStorageInfo.data.user.ID;
        userCode = localStorageInfo.data.user.Code;
        userFullName = localStorageInfo.data.user.FullName;

        //
        $(".toggle-th").click(function (event) {
            event.stopPropagation();

            $(".order-icon").removeClass("active");

            var id = $(this).attr("id");
            var icon = $("#" + id + " .order-icon");

            icon.text("⮟");
            icon.addClass("active");

            var name = icon.data('order-name');

            onLoadData(0, document.getElementById('hdfTypeOnLoad').value, "DESC", name);
        });

        $(".toggle-th .order-icon").click(function (event) {
            event.stopPropagation();

            var name = $(this).data('order-name');

            $(this).text(function (_, text) {
                if (text === "⮝") {
                    onLoadData(0, document.getElementById('hdfTypeOnLoad').value, "DESC", name);

                    return text = "⮟";
                } else {
                    onLoadData(0, document.getElementById('hdfTypeOnLoad').value, "ASC", name);

                    return text = "⮝";
                }
            });
        });

        $("#loader").hide();
    }
});



/*Logout*/
function returnLogin() {
    if (userID != null && userID !== "" && userID !== "undefined") {
        var result = "";
        var browserName = "";
        var browserVersion = "";
        var device = "";

        try {
            result = bowser.getParser(window.navigator.userAgent);
            browserName = result.parsedResult.browser.name;
            browserVersion = result.parsedResult.browser.version;
            device = result.parsedResult.os.name;
        } catch (e) {
            console.log(e);
        }


        var settings = {
            "url": common_api_url + "users/logout",
            "method": "POST",
            "timeout": 0,
            "headers": {
                "Authorization": token,
                "Content-Type": "application/json"
            },
            "data": JSON.stringify({
                "userID": userID,
                "browserName": browserName,
                "browserVersion": browserVersion,
                "device": device
            }),
        };

        $("#loader").show();

        $.ajax(settings).done(function (rs) {
            if (rs.code == 200) {
                console.log(rs);
                $("#loader").hide();
            } else {
                console.log(rs);
                $("#loader").hide();
            }
        });
    }

    storageClear();

    var urls = window.location.protocol + '//' + window.location.hostname + (window.location.port ? ':' + window.location.port : '');

    var urlLogin = urls + login_url;

    parent.window.location.href = urlLogin;
}



/*Check update*/
function checkForUpdate() {
    if (window.applicationCache != undefined && window.applicationCache != null) {
        window.applicationCache.addEventListener('updateready', updateApplication);
    }
}


function updateApplication(event) {
    if (window.applicationCache.status != 4) return;
    window.applicationCache.removeEventListener('updateready', updateApplication);
    window.applicationCache.swapCache();
    window.location.reload();
}



/*Local Storage*/
function storageGetItem(itemname) {
    return parent.window.localStorage.getItem(itemname);
}


function storageRemoveItem(itemname) {
    parent.window.localStorage.removeItem(itemname);
}


function storageClear() {
    storageRemoveItem("localStorageList");
    window.localStorage.clear();
}



/*Cookies*/
function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));

    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}


function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}



/*Format*/
function formatNumber(n) {
    try {
        var x = Math.round(n * 100) / 100;

        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    } catch (e) {
        return n;
    }
}


function formatDate(date, fm) {
    date = new Date(date);
    try {
        if (fm == "yyyy-mm-dd") {
            var dateFm = + date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate()));

            return dateFm;
        }

        if (fm == "yyyy-mm-dd hh:ss") {
            var dateFm = + date.getFullYear() + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + ' ' + (date.getHours() > 9 ? date.getHours() : '0' + date.getHours()) + ':' + (date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes());

            return dateFm;
        }

        if (fm == "mm-dd-yyyy") {
            var dateFm = ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' + date.getFullYear();
            return dateFm;
        }

        if (fm == "mm-dd-yyyy hh:ss") {
            var dateFm = ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' + date.getFullYear() + ' ' + (date.getHours() > 9 ? date.getHours() : '0' + date.getHours()) + ':' + (date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes());
            return dateFm;
        }

        if (fm == "dd-mm-yyyy") {
            var dateFm = ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + date.getFullYear();
            return dateFm;
        }

        if (fm == "dd/mm/yyyy") {
            var dateFm = ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '/' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '/' + date.getFullYear();
            return dateFm;
        }

        if (fm == "dd-mm-yyyy hh:ss") {
            var dateFm = ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + date.getFullYear() + ' ' + (date.getHours() > 9 ? date.getHours() : '0' + date.getHours()) + ':' + (date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes());
            return dateFm;
        }

        if (fm == "dd-mm") {
            var dateFm = ((date.getDate() > 9) ? date.getDate() : ('0' + date.getDate())) + '-' + ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1)));
            return dateFm;
        }

        if (fm == "hh:ss") {
            var dateFm = ((date.getHours() > 9 ? date.getHours() : '0' + date.getHours()) + ':' + (date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes()));
            return dateFm;
        }

        if (fm == "mm-yyyy") {
            var dateFm = ((date.getMonth() > 8) ? (date.getMonth() + 1) : ('0' + (date.getMonth() + 1))) + '-' + date.getFullYear();
            return dateFm;
        }

        return date.toLocaleString();
    } catch (e) {

        return date;
    }
}


function timeSince(seconds) {
    var interval = seconds / 31536000;

    if (interval > 1) {
        return Math.floor(interval) + " năm trước";
    }
    interval = seconds / 2592000;
    if (interval > 1) {
        return Math.floor(interval) + " tháng trước";
    }
    interval = seconds / 86400;
    if (interval > 1) {
        return Math.floor(interval) + " ngày trước";
    }
    interval = seconds / 3600;
    if (interval > 1) {
        return Math.floor(interval) + " giờ trước";
    }
    interval = seconds / 60;
    if (interval > 1) {
        return Math.floor(interval) + " phút trước";
    }
    return Math.floor(seconds) + " giây trước";
}


function timeSinceSpecial(seconds) {
    var interval = seconds / 31536000;

    if (interval > 1) {
        return Math.floor(interval) + " năm";
    }
    interval = seconds / 2592000;
    if (interval > 1) {
        return Math.floor(interval) + " tháng";
    }
    interval = seconds / 86400;
    if (interval > 1) {
        return Math.floor(interval) + " ngày";
    }
    interval = seconds / 3600;
    if (interval > 1) {
        return Math.floor(interval) + " giờ";
    }
    interval = seconds / 60;
    if (interval > 1) {
        return Math.floor(interval) + " phút";
    }
    return "< 1 phút";
}



/*Check String Is a Url*/
function isValidURL(string) {
    var res = string.match(/(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g);
    return (res !== null)
};



/*Check empty or white space*/
function checkEmptyOrWhiteSpace(str) {
    return (str.match(/^\s*$/) || []).length > 0;
}



/*Check validate value*/
function checkValidateValue(val, type, nameType) {
    if (type == 1) {
        if (val == null || val === "" || val === "undefined") {
            return true;
        }

        if (nameType === 'phone' && checkValidatePhone(val)) {
            return true;
        } else if (nameType === 'email' && checkValidateEmail(val)) {
            return true;
        }
    } else if (type == 2) {
        if (val != null && val !== "" && val !== "undefined") {
            if (nameType === 'phone' && checkValidatePhone(val)) {
                return true;
            } else if (nameType === 'email' && checkValidateEmail(val)) {
                return true;
            } else if (nameType === 'website' && checkValidateWebsite(val)) {
                return true;
            }
        }
    } else {
        if (val == 0 || val == null || val === "" || val === "undefined") {
            return true;
        }
    }
}



/*Check validate email*/
function checkValidateEmail(text) {
    var mailFormat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;

    if (mailFormat.test(text) == false) {
        return true;
    } else {
        return false;
    }
}



/*Check validate phone*/
function checkValidatePhone(text) {
    var phoneFormat = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;

    if (phoneFormat.test(text) == false) {
        return true;
    } else {
        return false;
    }
}



/*Check validate phone*/
function checkValidateWebsite(text) {
    var website = /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g;

    if (website.test(text) == false) {
        return true;
    } else {
        return false;
    }
}



/*Warning missing value input*/
function warningInvalidValue(dom, type, nameType) {
    var html = "";
    var title = "";

    if (type == 1) {
        if (nameType === 'text') {
            html += `<i class="fa-solid fa-circle-info me-1"></i>Vui lòng nhập đầy đủ thông tin`;
        } else if (nameType === 'email') {
            html += `<i class="fa-solid fa-circle-info me-1"></i>Email không đúng định dạng`;

            title = "Ví dụ: example@gmail.com";
        } else if (nameType === 'phone') {
            html += `<i class="fa-solid fa-circle-info me-1"></i>Số điện thoại không đúng định dạng`;

            title = "- Số điện thoại phải có ít nhất 10 ký tự số và tối đa 12 ký tự số\n- Ví dụ: 09xxxxxxxxx, +84xxxxxxxxx, (028)xxxxxxxx ...";
        } else if (nameType == 'file') {
            html += `<i class="fa-solid fa-circle-info me-1"></i>Dung lượng tệp tối đa là 2MB`;
        } else if (nameType == 'website') {
            html += `<i class="fa-solid fa-circle-info me-1"></i>Website không đúng định dạng`;

            title = "Ví dụ: www.example.com, https://example.com, ...";
        }

        $("#" + dom).focus(function () {
            $(this).css('border-color', '#066ac9');

            $("#" + dom + "Block").attr("title", title);
        });

        $("#" + dom).blur(function () {
            $(this).css('border-color', '#e8ebf3');

            $("#" + dom + "Block" + " .warning-invalid__title").hide();

            $("#" + dom + "Block").attr("title", "");

            $("#" + dom).focus(function () {
                $(this).css('border-color', '#85a8db');
            });
        });

        $("#" + dom).trigger("focus");
    } else if (type == 2) {
        html += `<i class="fa-solid fa-circle-info me-1"></i>Vui lòng chọn thông tin`;

        $("#" + dom).next('.select2').find('.select2-selection').one('focus', function () {
            $(".select2-container--focus").css({ 'border': '1px solid #066ac9', 'border-radius': '4px' });
        })

        $("#" + dom).next('.select2').find('.select2-selection').one('blur', function () {
            $(".select2-container--focus").css('border', 'unset');

            $("#" + dom + "Block" + " .warning-invalid__title").hide();
        })

        $("#" + dom).select2("focus");
    }

    $("#" + dom + "Block" + " .warning-invalid__title").html(html);
    $("#" + dom + "Block" + " .warning-invalid__title").show();
}



/*Auto complete*/
function initAutoComplete(inputSelector, nameAPI
    , getData, onSelectCallback, type, id) {
    $(inputSelector).autocomplete({
        source: function (request, response) {
            getData(request.term, nameAPI, function (rs) {
                if (rs.code == 200) {
                    var data = rs.data;

                    if (data.length > 0) {
                        response($.map(data, function (item) {
                            if (item.Code != null && item.Code !== "" && item.Code !== "undefined") {
                                if (item.ID != id) {
                                    return {
                                        label: item.Code + " - " + item.Name,
                                        value: item.Code + " - " + item.Name,
                                        data: item
                                    };
                                }
                            } else {
                                if (item.ID != id) {
                                    return {
                                        label: item.Name,
                                        value: item.Name,
                                        data: item
                                    };
                                }
                            }
                        }));
                    } else {
                        $(inputSelector + "Val").val("");
                    }

                } else {
                    alertError("Đã có lỗi xảy ra");
                }
            }, type);
        },
        minLength: 1,
        select: function (event, ui) {
            $(inputSelector).val(ui.item.value);
            onSelectCallback(ui.item);
        },
    });
}


function getDataByKey(data, nameAPI, callback, type) {
    var settings = {};

    if (type != null && type !== "" && type !== "undefined") {
        settings = {
            "url": api_url + nameAPI + "/auto-complete?key=" + data + "&type=" + type,
            "method": "GET",
            "timeout": 0,
            "headers": {
                "Authorization": token,
                "Content-Type": "application/json"
            },
        };
    } else {
        settings = {
            "url": api_url + nameAPI + "/auto-complete?key=" + data,
            "method": "GET",
            "timeout": 0,
            "headers": {
                "Authorization": token,
                "Content-Type": "application/json"
            },
        };
    }

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        $("#loader").hide();
        callback(rs);
    });
}



/*Limit string*/
function limitStringLength(numb, text) {
    if (text.length > numb) {
        var trimmedString = text.substring(0, numb) + "...";

        return trimmedString;
    } else {
        return text;
    }
}



/*Tree table*/
function flatListToTree(items) {
    const getChild = (item, level, allLevel) => {
        return items.filter(v => v.ParentID === item.ID)
            .map(v => {
                const temp = {
                    ...v,
                    level,
                    children: getChild(v, level + 1, level === 0 ? v.ID : `${allLevel}_${v.ID}`),
                    partLevel: level === 0 ? v.ID : `${v.ParentID}_${v.ID}`,
                    ...(level === 0 ? {
                        allLevel: v.ID
                    } : {
                        allLevel: [allLevel, v.ID].join('_')
                    }),
                };

                return [temp].concat(...temp.children);
            }
            );
    };

    return [].concat(...getChild({ ID: undefined }, 0, undefined))
};


$(document.body).delegate('.expand', 'click', function () {
    var level = $(this).attr('data-level');
    var partLevel = $(this).attr('data-part-level');
    var allLevel = $(this).attr('data-all-level');
    var isOpen = $(this).attr('data-is-open');
    var trsDiv = $('.tree-table').find('tbody tr');
    var trsArray = $(trsDiv);
    if (isOpen === '1') {
        for (var i = 0; i < trsArray.length - 1; i++) {
            var tempTr = $(trsArray[i]);
            var trLevel = tempTr.attr('data-level');
            var trPartLevel = tempTr.attr('data-part-level');
            var trAllLevel = tempTr.attr('data-all-level');
            var contain = trAllLevel.split('_')[Number(level)]; // 通过循环出来的tr的all_level获取选中等级的ID
            var curr = partLevel.split('_'); // 通过获取选中的part_level的最后一个元素获取选中等级的ID
            // 判断是否相等，
            if (contain && contain === curr[curr.length - 1] && partLevel !== trPartLevel) {
                tempTr.removeClass('show');
                tempTr.addClass('hIDden');
            }
        }
        $(this).text('+');
        $(this).attr('data-is-open', '0');
    } else {
        for (var i = 0; i < trsArray.length - 1; i++) {
            var tempTr = $(trsArray[i]);
            var trLevel = tempTr.attr('data-level');
            var trPartLevel = tempTr.attr('data-part-level');
            var trAllLevel = tempTr.attr('data-all-level');
            var contain = trAllLevel.split('_')[Number(level)]; // 通过循环出来的tr的all_level获取选中等级的ID
            var curr = partLevel.split('_'); // 通过获取选中的part_level的最后一个元素获取选中等级的ID
            // 判断是否相等，
            if (contain && contain === curr[curr.length - 1] && Number(trLevel) > (Number(level))) {
                var span = $(tempTr.children()[0].children[Number(trLevel)]);
                var isOpen = $(span).attr('data-is-open');
                var childrenCount = $(span).attr('data-count');
                tempTr.removeClass('hIDden');
                tempTr.addClass('show');
                // pLevel != -1 并且有子级的情况下，判断pLevel的开关状态，关闭则不展开其下级元素
                if (isOpen && isOpen === '0' && Number(childrenCount) > 0) { // 下级折叠状态
                    i = i + Number(childrenCount);
                } else {
                    if (isOpen === '1') {
                        $(span).attr('data-is-open', '1');
                        $(span).text('-');
                        tempTr.removeClass('hIDden');
                        tempTr.addClass('show');
                    }
                }
            }
        }
        $(this).text('-');
        $(this).attr('data-is-open', '1');
    }
});


function countChildren(node) {
    var sum = 0,
        children = node && node.length ? node : node.children,
        i = children && children.length;

    if (!i) {
        sum = 0;
    } else {
        while (--i >= 0) {
            if (node && node.length) {
                sum++;
                countChildren(children[i]);
            } else {
                sum += countChildren(children[i]);
            }
        }
    }
    return sum;
}



/*Format Number*/
function formatNumberInput(input) {
    var inputValue = input.value.replace(/[^\d]/g, '');

    var formattedValue = Number(inputValue).toLocaleString('en-US');

    input.value = formattedValue;
}


function getValueWithoutCommas(input) {
    var inputValue = input.replace(/,/g, '');

    var integerValue = parseInt(inputValue, 10);

    if (!isNaN(integerValue)) {
        return integerValue;
    } else {
        return "";
    }
}



/*Count day*/
function daysBetweenDates(date1, date2) {
    const oneDay = 1000 * 60 * 60 * 24;
    const differenceMs = Math.abs(date1 - date2);
    return Math.round(differenceMs / oneDay);
}


function viewDaysBetweenDates(domDate1, domDate2
    , domDateResult, domSetVal1, domSetVal2) {
    var date1 = $("#" + domDate1).val();

    var date2 = $("#" + domDate2).val();

    var days = "";

    if (date1 != null && date1 !== "" && date1 !== "undefined") {
        if (date1 < date2) {
            days = daysBetweenDates(new Date(date1), new Date(date2));

            $("#" + domDateResult).val(days);

            if (domSetVal1) {
                $("#" + domSetVal1).val(date1);

                $("#" + domSetVal2).val(date2);
            }
        } else {
            $("#" + domDateResult).val("Ngày bắt đầu phải trước ngày kết thúc");

            if (domSetVal1) {
                $("#" + domSetVal1).val("");

                $("#" + domSetVal2).val("");
            }
        }
    }
}



/*Administrative units*/
function onLoadCommonProvinces(domProvince, isDetail
    , detailProvinceID, detailDistrictID, detailWardID) {
    var optionHtml = "";

    var settings = {
        "url": api_url + "provinces",
        "method": "GET",
        "timeout": 0,
        "headers": {
            "Authorization": token
        },
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            var data = rs.data;
            var dataLength = data.length;

            optionHtml += `<option value="0">-- Chọn tỉnh/ thành --</option>`;

            for (var i = 0; i < dataLength; i++) {
                optionHtml += `<option value="${data[i].ID}">${data[i].Name}</option>`;
            }

            $("#" + domProvince).html(optionHtml);

            if (isDetail) {
                $("#" + domProvince).val(detailProvinceID).trigger("change");

                var districtSelect = $("#" + domProvince).closest(".form-group").next().find("select");
                var wardSelect = districtSelect.closest(".form-group").next().find("select");

                onLoadCommonDistricts(domProvince, districtSelect.attr("id"), wardSelect.attr("id"), true, detailDistrictID, detailWardID);
            }

            $("#loader").hide();
        } else if (rs.code == 310) {
            returnLogin();
        } else {
            $("#loader").hide();
            console.log(rs);
            alertError(rs.messVN);
        }
    });
}


function onLoadCommonDistricts(domProvince, domDistrict
    , domWard, isDetail, detailDistrictID, detailWardID) {
    var optionHtml = "";
    var province = $("#" + domProvince).val();

    if (isDetail && typeof isDetail === "string") {
        $("#" + isDetail).val(province).trigger("change");
    }

    if (province != 0 && province != null) {
        var settings = {
            "url": api_url + "districts?provinceId=" + province,
            "method": "GET",
            "timeout": 0,
            "headers": {
                "Authorization": token
            },
        };

        $("#loader").show();

        $.ajax(settings).done(function (rs) {
            if (rs.code == 200) {
                var data = rs.data;
                var dataLength = data.length;

                optionHtml += `<option value="0">-- Chọn quận/ huyện --</option>`;

                for (var i = 0; i < dataLength; i++) {
                    optionHtml += `<option value="${data[i].ID}">${data[i].Name}</option>`;
                }

                $("#" + domDistrict).html(optionHtml);

                if (isDetail && typeof isDetail === "boolean") {
                    $("#" + domDistrict).val(detailDistrictID).trigger("change");

                    onLoadCommonWards(domDistrict, domWard, isDetail, detailWardID);
                } else {
                    $("#" + domWard).html("");
                }

                $("#loader").hide();
            } else if (rs.code == 310) {
                returnLogin();
            } else {
                $("#loader").hide();
                console.log(rs);
                alertError(rs.messVN);
            }
        });
    } else {
        $("#" + domDistrict).html(optionHtml);
        $("#" + domWard).html(optionHtml);
    }
}


function onLoadCommonWards(domDistrict, domWard
    , isDetail, detailWardID) {
    var optionHtml = "";
    var district = $("#" + domDistrict).val();

    if (isDetail && typeof isDetail === "string") {
        $("#" + isDetail).val(district).trigger("change");
    }

    if (district != 0 && district != null) {
        var settings = {
            "url": api_url + "wards?districtId=" + district,
            "method": "GET",
            "timeout": 0,
            "headers": {
                "Authorization": token
            },
        };

        $("#loader").show();

        $.ajax(settings).done(function (rs) {
            if (rs.code == 200) {
                var data = rs.data;
                var dataLength = data.length;

                optionHtml += `<option value="0">-- Chọn phường/ xã --</option>`;

                for (var i = 0; i < dataLength; i++) {
                    optionHtml += `<option value="${data[i].ID}">${data[i].Name}</option>`;
                }

                $("#" + domWard).html(optionHtml);

                if (isDetail && typeof isDetail === "boolean") {
                    $("#" + domWard).val(detailWardID).trigger("change");
                }

                $("#loader").hide();
            } else if (rs.code == 310) {
                returnLogin();
            } else {
                $("#loader").hide();
                console.log(rs);
                alertError(rs.messVN);
            }
        });
    } else {
        $("#" + domWard).html(optionHtml);
    }
}


function onLoadCommonMore(dom, domMore, numb) {
    var domVal = $("#" + dom).val();

    if (numb == 1) {
        $("#" + domMore).val(domVal);
    } else {
        $("#" + domMore).val(domVal).trigger("change");
    }
}


/*Dom handle*/
function disableDom(dom) {
    $("#" + dom).prop('disabled', true);
}


function removeDisableDom(dom) {
    $("#" + dom).prop('disabled', false);
}



/*Assign block*/
function openAssignBlock() {
    $("#txtDetailAssign").val(userCode + " - " + userFullName);

    $("#txtDetailAssignVal").val(userID);
    
    initAutoComplete("#txtDetailAssign", "users", getDataByKey, function (selectedItem) {
        $("#txtDetailAssignVal").val(selectedItem.data.ID);
    });

    $(".detail-sub-block").hide();

    $("#assignBlock").show();
}


function closeAssignBlock() {
    $("#txtDetailAssign").val(userCode + " - " + userFullName);

    $("#txtDetailAssignVal").val(userID);

    $("#assignBlock").hide();
}



/*Notes*/
function viewCreateNotes(key) {
    if (key) {
        $("#createNoteBtn").hide();
        $("#createNoteBlock").show();
    } else {
        $("#createNoteBlock").hide();
        $("#createNoteBtn").show();

        clearAllForm("createNoteBlock");
    }
}


function onLoadDataNotes() {
    $('#loader').show();

    var id = $("#hdfDetailID").val();

    $.ajax({
        url: common_api_url + 'notes/list',
        type: "POST",
        data: { ID: id },
        success: function (rs) {
            var html = "";

            if (rs.code == 200) {
                var data = rs.data;
                var dataLength = data.length;

                if (dataLength > 0) {
                    for (var i = 0; i < dataLength; i++) {
                        if (rs.files) {
                            var listFiles = '';

                            var files = rs.files.filter(x => x.NoteID == data[i].ID);

                            var fileLength = files.length;

                            if (fileLength > 0) {
                                for (var j = 0; j < fileLength; j++) {
                                    var fileName = limitStringLength(24, files[j].FileName);

                                    var file = `<a download="${files[j].FileName}" target="_blank" href="${files[j].FileBase64}"><i class="fas fa-paperclip"></i> ${fileName}</a>`;

                                    file += `<a onclick="submitDeleteNoteFile(\`` + files[j].ID + `\`)" title="Xóa tệp tin" class="ms-2" href="javascript:void(0);"><b><i class="fas fa-times text-danger"></i></b></a> <br />`;

                                    listFiles += file;
                                }
                            } else {
                                var emptyFile = `<form id="frmDetailAttachment" enctype="multipart/form-data" method="POST">`;

                                emptyFile += `<input id="txtDetailNoteFile" name="txtDetailNoteFile" type="file" multiple class="form-control"></form>`;

                                listFiles += emptyFile;
                            }
                        }

                        html += `<li id="${data[i].ID}-block" class="note-list__item p-3 mb-4">`;
                        html += `<div onclick="submitDeleteNote(\`` + data[i].ID + `\`)" class="text-end"><button title="Xóa" type="button" class="btn-close" aria-label="Close"></button></div>`;

                        html += `<div id="${data[i].ID}-inputBlock" class="form-floating mb-1">`;
                        html += `<input id="${data[i].ID}-input" value="${(data[i].Title != null ? data[i].Title : '-')}" type="text" spellcheck="false" autocomplete="off" class="form-control fw-bold" />`;
                        html += `<div class="warning-invalid__title mt-1"></div>`;
                        html += `<label for="${data[i].ID}-input">Title</label>`;
                        html += `</div>`;

                        html += `<div class="form-floating mb-2">`;
                        html += `<textarea id="${data[i].ID}-textarea" spellcheck="false" rows="2" class="form-control">${(data[i].Content != null ? data[i].Content : '-')}</textarea>`;
                        html += `<label for="${data[i].ID}-textarea">Content</label>`;
                        html += `</div>`;

                        html += `<div title="Tải xuống" class="mb-2">${listFiles}</div>`;

                        html += `<div class="d-flex align-items-center justify-content-between mb-2">`;
                        html += `<div class="table-detail">${(data[i].CreatedByName != null ? data[i].CreatedByName : '-')}</div>`;
                        html += `<div>${(data[i].CreatedOn != null ? formatDate(data[i].CreatedOn, 'dd-mm-yyyy hh:ss') : '-')}</div>`;
                        html += `</div>`;

                        html += `<div class="d-flex align-items-center justify-content-end">`;
                        html += `<div onclick="submitUpdateNoteCommon(\`` + data[i].ID + `\`)" class="cursor-pointer btn-note p-2">Done <i class="fa-solid fa-check"></i></div>`;
                        html += `</div>`;

                        html += `</li>`;
                    }
                } else {
                    html += `<li class="note-list__item p-3">Không tìm thấy dữ liệu</li>`;
                }

                $("#dataNoteList").html(html);

                $("#loader").hide();
            } else if (rs.code == 310) {
                returnLogin();
            } else {
                console.log(rs);
                alertError(rs.messVN);
                $("#loader").hide();
            }
        }
    });
}


function submitCreateNoteCommon() {
    var entityID = $("#hdfDetailID").val();
    var title = $('#txtNoteTitle').val();
    var content = $('#txtNoteContent').val();

    var validationChecks = [];

    validationChecks = [
        { input: title, type: 1, element: 'text', name: 'txtNoteTitle' }
    ];

    for (const check of validationChecks) {
        if (checkValidateValue(check.input, check.type)) {
            if (check.element) {
                warningInvalidValue(check.name, check.type, check.element);
            }
            else {
                warningInvalidValue(check.name, 2);
            }

            return;
        }
    }

    $('#loader').show();

    var data = {
        EntityID: entityID,
        Title: title,
        Content: content,
        CreatedBy: common_get_myid(),
        ModifiedBy: common_get_myid(),
    };

    var frmData = new FormData(document.getElementById('frmAttachment'));

    frmData.append('Note', JSON.stringify(data));

    $.ajax({
        url: common_api_url + 'notes',
        type: "POST",
        data: frmData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (rs) {
            if (rs.code == 200) {
                alertSuccess(rs.messVN);
                onLoadDataNotes();
                clearAllForm("createNoteBlock");
                viewCreateNotes();
                $('#loader').hide();
            } else if (rs.code == 310) {
                returnLogin();
            } else {
                console.log(rs);
                alertError(rs.messVN);
                $("#loader").hide();
            }
        }
    });
}


function submitUpdateNoteCommon(id) {
    if (id) {
        var entityID = $("#hdfDetailID").val();
        var title = $("#" + id + "-input").val();
        var content = $('#' + id + "-textarea").val();

        var validationChecks = [];

        validationChecks = [
            { input: title, type: 1, element: 'text', name: id + "-input" }
        ];

        for (const check of validationChecks) {
            if (checkValidateValue(check.input, check.type)) {
                if (check.element) {
                    warningInvalidValue(check.name, check.type, check.element);
                }
                else {
                    warningInvalidValue(check.name, 2);
                }

                return;
            }
        }

        $('#loader').show();

        var data = {
            ID: id,
            EntityID: entityID,
            Title: title,
            Content: content,
            CreatedBy: common_get_myid(),
            ModifiedBy: common_get_myid(),
        };

        var frmData = "";
        var frm = document.getElementById('frmDetailAttachment');

        if (frm) {
            frmData = new FormData(frm);
        } else {
            frmData = new FormData();
        }

        frmData.append('Note', JSON.stringify(data));

        $.ajax({
            url: common_api_url + 'notes',
            type: "PUT",
            data: frmData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (rs) {
                if (rs.code == 200) {
                    alertSuccess(rs.messVN);
                    onLoadDataNotes();
                    $('#loader').hide();
                } else if (rs.code == 310) {
                    returnLogin();
                } else {
                    console.log(rs);
                    alertError(rs.messVN);
                    $("#loader").hide();
                }
            }
        });
    }
}


function submitDeleteNote(id) {
    swal({
        text: "Bạn muốn xóa dữ liệu?",
        buttons: true,
        buttons: ["Hủy", "Đồng ý"],
        dangerMode: true,
    })
        .then((doit) => {
            if (doit) {
                $("#loader").show();

                $.ajax({
                    url: common_api_url + 'notes',
                    type: "DELETE",
                    data: { ID: id },
                    success: function (rs) {
                        if (rs.code == 200) {
                            alertSuccess(rs.messVN);
                            onLoadDataNotes();
                            $("#loader").hide();
                        } else if (rs.code == 310) {
                            returnLogin();
                        } else {
                            console.log(rs);
                            alertError(rs.messVN);
                            $("#loader").hide();
                        }
                    }
                });
            }
        });
}


function submitDeleteNoteFile(id) {
    swal({
        text: "Bạn muốn xóa dữ liệu?",
        buttons: true,
        buttons: ["Hủy", "Đồng ý"],
        dangerMode: true,
    })
        .then((doit) => {
            if (doit) {
                $("#loader").show();

                var data = {
                    ID: id,
                    EntityID: $('#hdfDetailID').val(),
                };

                $.ajax({
                    url: common_api_url + 'notes/files',
                    type: "DELETE",
                    data: data,
                    success: function (rs) {
                        if (rs.code == 200) {
                            alertSuccess(rs.messVN);
                            onLoadDataNotes();
                            $("#loader").hide();
                        } else if (rs.code == 310) {
                            returnLogin();
                        } else {
                            console.log(rs);
                            alertError(rs.messVN);
                            $("#loader").hide();
                        }
                    }
                });
            }
        });
}



/*Task*/
function viewCreateTasks(key) {
    if (key) {
        $("#createTaskBtn").hide();
        $("#createTaskBlock").show();

        $("#txtTaskOwnerVal").val(userID);
        $("#txtTaskOwner").val(userCode + " - " + userFullName);
    } else {
        $("#createTaskBlock").hide();
        $("#createTaskBtn").show();
        clearAllForm("createTaskBlock");
    }
}


function onLoadDataTasks() {
    var id = $("#hdfDetailID").val();

    var settings = {
        "url": api_url + "tasks/list",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Authorization": token,
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "isAll": true,
            "id": id
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            var html = "";
            var data = rs.data;
            var dataLength = data.length;

            if (dataLength > 0) {
                for (var i = 0; i < dataLength; i++) {
                    var subject = "";
                    if (data[i].Subject == null || data[i].Subject === "undefined" || data[i].Subject === "") {
                        subject = "-";
                    } else {
                        subject = limitStringLength(16, data[i].Subject)
                    }

                    var priority = "";
                    if (data[i].Priority == null || data[i].Priority === "undefined" || data[i].Priority === "") {
                        priority = "-";
                    } else {
                        if (data[i].Priority == 1) {
                            priority = "Rất thấp";
                        } else if (data[i].Priority == 2) {
                            priority = "Thấp";
                        } else if (data[i].Priority == 3) {
                            priority = "Trung bình";
                        } else if (data[i].Priority == 4) {
                            priority = "Cao";
                        } else {
                            priority = "Rất cao";
                        }
                    }

                    if (data[i].ModifiedByName == null || data[i].ModifiedByName === "undefined" || data[i].ModifiedByName === "") {
                        data[i].ModifiedByName = "-";
                    }

                    if (data[i].CompletedByName == null || data[i].CompletedByName === "undefined" || data[i].CompletedByName === "") {
                        data[i].CompletedByName = "-";
                    }

                    var modifiedOn = "";
                    if (data[i].ModifiedOn == null || data[i].ModifiedOn === "undefined" || data[i].ModifiedOn === "") {
                        modifiedOn = "-";
                    } else {
                        modifiedOn = formatDate(new Date(data[i].ModifiedOn), "dd-mm-yyyy hh:ss");
                    }

                    var completedOn = "";
                    if (data[i].CompletedOn == null || data[i].CompletedOn === "undefined" || data[i].CompletedOn === "") {
                        completedOn = "-";
                    } else {
                        completedOn = formatDate(new Date(data[i].CompletedOn), "dd-mm-yyyy hh:ss");
                    }

                    html += `<li id="${data[i].ID}-block" class="note-list__item p-3 mb-4">`;

                    html += `<div>`;
                    html += `<div onclick="submitDeleteTask(\`` + data[i].ID + `\`)" class="text-end"><button title="Xóa" type="button" class="btn-close" aria-label="Close"></button></div>`;

                    if (data[i].Status == 0) {
                        html += `<div class="d-flex align-items-center mb-2">`;
                        html += `<div class="col-md-4"><i class="font-size-1-4 fa-solid fa-people-arrows p-3 border"></i></div>`;
                        html += `<div class="col-md-8 d-flex flex-column">`;
                        html += `<span class="fw-bold">${subject}</span>`;
                        html += `<span>Priority: ${priority}</span>`;
                        html += `<span>Modified by: <span class="table-detail">${data[i].ModifiedByName}</span></span>`;
                        html += `<span class="text-end">${modifiedOn}</span>`;
                        html += `</div>`;
                        html += `</div>`;

                        html += `<div class="d-flex align-items-center justify-content-between">`
                        html += `<span onclick="submitUpdateTaskCommon(\`` + data[i].ID + `\`)" title="Hoàn thành" class="table-detail">COMPLETE</span>`;
                        html += `<i title="Xem chi tiết" class="cursor-pointer font-size-1-2 fa-solid fa-arrow-up-right-from-square"></i>`;
                        html += `</div>`;
                        html += `</div>`;  
                    } else {
                        html += `<div class="d-flex align-items-center mb-2">`;
                        html += `<div class="col-md-4"><i class="font-size-1-4 fa-solid fa-people-arrows p-3 border"></i></div>`;
                        html += `<div class="col-md-8 d-flex flex-column">`;
                        html += `<span class="fw-bold">${subject}</span>`;
                        html += `<span>Priority: ${priority}</span>`;
                        html += `<span>Completed by: <span class="table-detail">${data[i].CompletedByName}</span></span>`;
                        html += `<span class="text-end">${completedOn}</span>`;
                        html += `</div>`;
                        html += `</div>`;

                        html += `<div class="text-end">`
                        html += `<i title="Xem chi tiết" class="cursor-pointer font-size-1-2 fa-solid fa-arrow-up-right-from-square"></i>`;
                        html += `</div>`;
                        html += `</div>`;  
                    }

                    html += `</li>`;
                }
            } else {
                html += `<li class="note-list__item p-3">Không tìm thấy dữ liệu</li>`;
            }

            $("#dataTaskList").html(html);

            $("#loader").hide();
        } else if (rs.code == 310) {
            returnLogin();
        } else {
            console.log(rs);
            alertError(rs.messVN);
            $("#loader").hide();
        }
    });
}


function submitCreateTaskCommon() {
    var entityID = $("#hdfDetailID").val();
    var subject = $("#txtTaskSubject").val();
    var description = $("#txtTaskDescription").val();
    var due = $("#txtTaskDue").val();
    var priority = $("#ddlTaskPriority").val();
    var owner = $("#txtTaskOwnerVal").val();

    var validationChecks = [];

    validationChecks = [
        { input: subject, type: 1, element: 'text', name: 'txtTaskSubject' },
        { input: owner, type: 1, element: 'text', name: 'txtTaskOwner' },
    ];

    for (const check of validationChecks) {
        if (checkValidateValue(check.input, check.type)) {
            if (check.element) {
                warningInvalidValue(check.name, check.type, check.element);
            }
            else {
                warningInvalidValue(check.name, 2);
            }

            return;
        }
    }

    var settings = {
        "url": api_url + "tasks",
        "method": "POST",
        "timeout": 0,
        "headers": {
            "Authorization": token,
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "EntityID": entityID,
            "OwnerID": owner,
            "Subject": subject,
            "Description": description,
            "Due": due,
            "Priority": priority,
            "CreatedBy": userID,
            "ModifiedBy": userID
        }),
    };

    $("#loader").show();

    $.ajax(settings).done(function (rs) {
        if (rs.code == 200) {
            alertSuccess(rs.messVN);
            onLoadDataTasks();
            clearAllForm("createTaskBlock");
            viewCreateTasks();
            $('#loader').hide();
        } else if (rs.code == 310) {
            returnLogin();
        } else {
            console.log(rs);
            alertError(rs.messVN);
            $("#loader").hide();
        }
    });
}


function submitUpdateTaskCommon(id) {
    swal({
        text: "Bạn muốn hoàn thành công việc này?",
        buttons: true,
        buttons: ["Hủy", "Đồng ý"],
        dangerMode: true,
    })
        .then((doit) => {
            if (doit) {
                var settings = {
                    "url": api_url + "tasks",
                    "method": "PUT",
                    "timeout": 0,
                    "headers": {
                        "Authorization": token,
                        "Content-Type": "application/json"
                    },
                    "data": JSON.stringify({
                        "ID": id,
                        "Status": 1,
                        "CompletedBy": userID,
                        "ModifiedBy": userID
                    }),
                };

                $("#loader").show();

                $.ajax(settings).done(function (rs) {
                    if (rs.code == 200) {
                        alertSuccess(rs.messVN);
                        onLoadDataTasks();
                        $("#loader").hide();
                    } else if (rs.code == 310) {
                        returnLogin();
                    } else {
                        console.log(rs);
                        alertError(rs.messVN);
                        $("#loader").hide();
                    }
                });
            }
        });
}


function submitDeleteTask(id) {
    var data = [];

    if (id) {
        data.push(id);
    }

    if (data.length == 0) {
        alertWarning("Vui lòng chọn thông tin");
        return;
    }
    else {
        swal({
            text: "Bạn muốn xóa dữ liệu?",
            buttons: true,
            buttons: ["Hủy", "Đồng ý"],
            dangerMode: true,
        })
            .then((doit) => {
                if (doit) {
                    var dataInput = {
                        "ID": data,
                    }

                    var settings = {
                        "url": api_url + "tasks",
                        "method": "DELETE",
                        "data": JSON.stringify(dataInput),
                        "timeout": 0,
                        "headers": {
                            "Authorization": token,
                            "Content-Type": "application/json"
                        },
                    };

                    $("#loader").show();

                    $.ajax(settings).done(function (rs) {
                        if (rs.code == 200) {
                            alertSuccess(rs.messVN);
                            onLoadDataTasks();
                            $("#loader").hide();
                        } else if (rs.code == 310) {
                            returnLogin();
                        } else {
                            console.log(rs);
                            alertError(rs.messVN);
                            $("#loader").hide();
                        }
                    });
                }
            });
    }
}



/*Alert*/
function alertError(message) {
    try {
        toastr.error("", message, { timeOut: 3000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}


function alertSuccess(message) {
    try {
        toastr.success("", message, { timeOut: 3000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}


function alertWarning(message) {
    try {
        toastr.warning("", message, { timeOut: 3000 });
    } catch (e) {
        console.log(e);
        alert(message);
    }
}



function common_fill_dropdown(id, data) {
    try {
        $('#' + id).html('');
        if (data.length > 0) {
            var option = `<option>---</option>`;
            for (var i = 0; i < data.length; i++) {
                option += `<option value="${data[i].ID}">${data[i].Name}</option>`;
            }
            $('#' + id).append(option);
            $('#' + id).select2();
        }
    } catch (e) {

    }
}

function common_district_by_province() {
    try {
        var ProvinceID = $('#ddlProvince option:selected').val();
        if (ProvinceID != '') {

            $.ajax({
                url: common_api_url + 'MasterData/DistByProv',
                type: "POST",
                data: { ProvinceID: ProvinceID },
                success: function (rs) {
                    $('#ddlDistrict').html('');
                    $('#ddlWard').html('');
                    if (rs.Data != undefined) {
                        common_fill_dropdown('ddlDistrict', rs.Data.map(x => ({ ID: x.DistrictID, Name: x.DistrictName })))
                    }
                }
            });

        }
    } catch (e) {

    }
}

function common_ward_by_dist_prov() {
    try {
        var ProvinceID = $('#ddlProvince option:selected').val();
        var DistrictID = $('#ddlDistrict option:selected').val();

        if (ProvinceID != '' && DistrictID != '') {
            $.ajax({
                url: common_api_url + 'MasterData/WardByDistProv',
                type: "POST",
                data: { ProvinceID: ProvinceID, DistrictID: DistrictID },
                success: function (rs) {
                    console.log(rs)
                    if (rs.Data != undefined) {
                        common_fill_dropdown('ddlWard', rs.Data.map(x => ({ ID: x.WardID, Name: x.WardName })))
                    }
                }
            });
        }
    } catch (e) {

    }
}


function common_verify_phone(id) {
    var error = '';
    var value = $('#' + id).val();
    var isNum = $.isNumeric(value);

    if (value != "" && (!isNum || value.length < 10)) {
        error = 'Số điện thoại không hợp lệ'
    }
    return error;
}

function common_verify_email(id) {
    var error = '';
    var value = $('#' + id).val();

    if (value != "") {
        if (value.indexOf(".") != -1 && value.indexOf("@@") != -1) {
            error = '';
        }
        else {
            error = 'Email không hợp lệ!';
        }
    }
    return error;
}

function common_get_myid() {
    if (localStorage.getItem("localStorageList") != null) {
        var list = JSON.parse(localStorage.getItem("localStorageList"));
        if (list.data.user.ID != undefined) {
            return list.data.user.ID;
        }
        else return common_guid_empty;
    }
    else return common_guid_empty;
}

function view_opportunity(id) {
    parent.indexPageOpportunityDetail(id);
}

function view_account(id) {
    parent.indexPageAccountDetail(id);
}

function view_contact(id) {
    parent.indexPageContactDetail(id);
}


function NoteUpdate() {
    try {
        $('#loader').show();
        var data = {
            EntityID: $('#hdfID').val(),
            Note: $('#txtNote').val(),
            CreatedBy: common_get_myid(),
        };
        var frmData = new FormData(document.getElementById('frmAttachment'));
        frmData.append('LeadNote', JSON.stringify(data));

        $.ajax({
            url: common_api_url + 'Note/Update',
            type: "POST",
            data: frmData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (rs) {
                $('#loader').hide();
                if (rs.Error != undefined) {
                    alertError(rs.Error);
                }
                document.getElementById('fileUpload').value = '';
                $('#txtNote').val('');
                NoteList();
            }
        });

    } catch (e) {

    }
}

function NoteDelete(ID) {
    try {
        var cf = confirm('Are you sure delete item?');
        if (cf) {
            $.ajax({
                url: common_api_url + 'Note/Delete',
                type: "POST",
                data: { ID: ID },
                success: function (rs) {
                    NoteList();
                }
            });
        }
    } catch (e) { }
}

function NoteList() {
    try {
        $('#loader').show();
        $.ajax({
            url: common_api_url + 'Note/List',
            type: "POST",
            data: { ID: $('#hdfID').val() },
            success: function (rs) {
                $('#loader').hide();
                $('#tblAttach').html('');

                if (rs.Data != undefined) {
                    for (var i = 0; i < rs.Data.length; i++) {

                        var iconDel = `<button class="btn btn-sm ivg-button-danger" onclick="NoteDelete('${rs.Data[i].ID}')"><i class="fas fa-trash"></i></button>`;

                        var listFiles = '';

                        if (rs.Files != undefined) {
                            var files = rs.Files.filter(x => x.NoteID == rs.Data[i].ID);
                            if (files != undefined && files.length > 0) {
                                for (var j = 0; j < files.length; j++) {
                                    var file = `<a download="${files[j].FileName}" target="_blank" href="${files[j].FileBase64}""><i class="fas fa-paperclip"></i> ${files[j].FileName}</a>`
                                    file += `<a title="Click to delete file" href="javascript:void(0);" onclick="NoteDeleteFile('${files[j].ID}')"><b><i class="fas fa-times text-danger"></i></b></a> <br />`
                                    listFiles += file;
                                }
                            }
                        }
                        var html = ``;
                        html += `<tr>`;
                        html += `<td>${(rs.Data[i].Note != null ? rs.Data[i].Note : '')}</td>`;
                        html += `<td>${listFiles}</td>`;
                        html += `<td>${(rs.Data[i].CreatedOn != null ? formatDate(rs.Data[i].CreatedOn, 'yyyy-mm-dd hh:ss') : '')}</td>`;
                        html += `<td>${iconDel}</td>`;
                        html += `</tr>`;

                        $('#tblAttach').append(html);
                    }
                }
            }
        });
    } catch (e) {}
}

function NoteDeleteFile(ID) {
    try {
        var cf = confirm('Are you sure delete item?');
        if (cf) {
            $('#loader').show();
            var data = {
                ID: ID,
                EntityID: $('#hdfID').val(),
            };
            $.ajax({
                url: common_api_url + 'Note/DeleteFile',
                type: "POST",
                data: data,
                success: function (rs) {
                    $('#loader').hide();
                }
            });
        }
    } catch (e) { }
}
