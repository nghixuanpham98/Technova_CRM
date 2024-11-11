var pr_userid = '';
var pr_zaloid = '';
BindInfo()

function BindInfo() {

    var queryString = window.location.search;

    queryString = queryString.toLowerCase();

    var urlParams = new URLSearchParams(queryString);

    var userid = urlParams.get('userid');
    var zaloid = urlParams.get('zaloid');


    console.log(`BindInfo userid`, userid);
    console.log(`BindInfo zaloid`, zaloid);


    if (userid != undefined && userid != 'null' && userid != '') {
        pr_userid = userid;

        UserStatus();
    }
    if (zaloid != 'null') {
        //alert(zaloid)
        pr_zaloid = zaloid;
        chat_detail(zaloid);
        //chat_detail_by_zaloid(zaloid);

    }
    else {
        conversation();

    }
}
$(function () {
    var timer = null, timer2 = null,
        interval = 1000, interval2 = 2000,
        value = 0;

    $("#start").click(function () {

        if (timer !== null) return;
        timer = setInterval(function () {
            $("#input").val(++value);
            heartbeat();
        }, interval);

        //if (timer2 !== null) return;
        //timer2 = setInterval(function () {
        //    conversation_recall();
        //}, interval2);
    });

    $("#stop").click(function () {
        clearInterval(timer);
        //clearInterval(timer2);
        timer = null;
        //timer2 = null;
    });
});

window.onload = function (e) {
    $("#start").click();
}
//setInterval(heartbeat, 2000);

function heartbeat() {
    try {

        var user_id = $('#hdfUserId').val();

        if (user_id == '' || user_id == undefined || user_id == null) return;

        var last_message_id = $('#hdfLastMsgId').val();

        $.ajax({
            url: api_url + 'chat_detail_recall',
            type: "POST",
            data: {
                user_id: user_id,
                last_message_id: last_message_id
            },
            success: function (rs) {
                $("#stop").click();
                draw_chat(rs);
                $("#start").click();
                //console.log(`chat_detail_recall`, rs);
            }
        });
    } catch (e) { }
}

function UserStatus() {
    try {
        var checked = $('#ckUserStatus').prop('checked');

        var data = {
            user_id: pr_userid,
            status: checked
        }
        $('.iconLoading').show();
        var settings = {
            url: api_url + 'user_status',
            type: "POST",
            data: data
        }
        $.ajax(settings).done(function (rs) {
            $('.iconLoading').hide();
            if (rs.stt != undefined) {
                var ck = rs.stt.Status == true ? true : false;
                $('#ckUserStatus').prop('checked', ck);
                if (ck) {
                    $('#lblStatusText').text('Online');
                    $('#lblStatusText').addClass('text-primary');
                } else {
                    $('#lblStatusText').text('Offline');
                    $('#lblStatusText').addClass('text-secondary');
                }
            }
        });

    } catch (e) { }
}


function conversation() {
    try {
        empty_input();
        $.ajax({
            url: api_url + 'conversation',
            type: "GET",
            data: { userid: pr_userid },
            success: function (rs) {
                if (rs.data != undefined) {

                    var template = $('#tmpChatList').html();

                    $('#pnlChatList').html('');

                    for (var i = 0; i < rs.data.length; i++) {
                        var data = rs.data[i];

                        var msg = JSON.parse(data.Message); //chỉ có nội dung này là khác nhau

                        if (msg.text != undefined && msg.text != '') {
                            msgContent = msg.text;
                        }
                        else if (msg.attachments != undefined && msg.attachments.length == 1) {
                            msgContent = `${msg.attachments[0].type}`;
                        }

                        var html = template.replace('@avatar', data.Avatar);
                        html = html.replace('@name', data.DisplayName);
                        html = html.replace('@user_id', data.UserId);
                        html = html.replace('@message', msgContent);
                        $('#pnlChatList').append(html);
                    }
                    if (rs.data.length == 0) {
                        html = `Không có dữ liệu`;
                        $('#pnlChatList').append(html);
                    }
                    HidePanel();
                    $('#pageChatList').show(200);
                }
            }
        });
    } catch (e) { }
}

function conversation_recall() {
    try {
        empty_input();
        $.ajax({
            url: api_url + 'conversation',
            type: "GET",
            data: { userid: pr_userid },
            success: function (rs) {
                if (rs.data != undefined) {

                    var template = $('#tmpChatList').html();

                    $('#pnlChatList').html('');

                    for (var i = 0; i < rs.data.length; i++) {
                        var data = rs.data[i];

                        var msg = JSON.parse(data.Message); //chỉ có nội dung này là khác nhau

                        if (msg.text != undefined && msg.text != '') {
                            msgContent = msg.text;
                        }
                        else if (msg.attachments != undefined && msg.attachments.length == 1) {
                            msgContent = `${msg.attachments[0].type}`;
                        }

                        var html = template.replace('@avatar', data.Avatar);
                        html = html.replace('@name', data.DisplayName);
                        html = html.replace('@user_id', data.UserId);
                        html = html.replace('@message', msgContent);
                        $('#pnlChatList').append(html);
                    }
                    if (rs.data.length == 0) {
                        html = `Không có dữ liệu`;
                        $('#pnlChatList').append(html);
                    }
                    HidePanel(1);
                    $('#pageChatList').show();
                }
            }
        }).done(function () {
        });
    } catch (e) { }
}

function empty_input() {
    $('#hdfUserId').val('');
    $('#hdfLastMsgId').val('');
    $('#hdfSessionName').val('');
}



sticker();
function sticker() {
    try {

        $.ajax({
            url: api_url + 'sticker',
            type: "GET",
            success: function (rs) {
                $('#sticker').attr('src', rs[0].src);
                var template = $('#tmpSticker').html();
                $('#pnlSticker').html('');
                for (var i = 0; i < rs.length; i++) {
                    var data = rs[i];
                    var html = template
                    html = html.replace('@src', data.src);
                    html = html.replace('@id', data.id);
                    $('#pnlSticker').append(html)
                }
            }
        });
    } catch (e) { }
}
function sticker_show() {
    $('#btnSticker').click();
}
function upload_file() {

    try {
        //var input = qa_get_info_save_db();

        //console.log(`qa_save`, input);

        //var frmData = new FormData();

        //frmData.append('QAInput', JSON.stringify(input));

        //$.ajax({
        //    url: api_uat_claim + 'qa_save',
        //    type: "POST",
        //    data: frmData,
        //    cache: false,
        //    contentType: false,
        //    processData: false,
        //    success: function (rs) {
        //        console.log(`qa_save rs`, rs)
        //    }
        //});

    } catch (e) {

    }
}

function chat_detail(user_id) {
    try {
        user_id = user_id != undefined ? user_id : $('hdfUserId').val();
        $('.iconLoading').show();

        $.ajax({
            url: api_url + 'chat_detail?user_id=' + user_id,
            type: "GET",
            success: function (rs) {
                $("#stop").click();
                console.log(`chat_detail`, rs);

                $('#pnlChatDetail').html('');

                if (rs.data != undefined) {

                    draw_chat(rs, 1);

                    //kiểm tra xem người đang đăng nhập có thuộc owner này để chat tiếp không

                    if (rs.sessionList != undefined) {
                        var sessionOpen = rs.sessionList.filter(x => x.Status == 1);
                        if (sessionOpen != undefined && sessionOpen.length == 1) {
                            var ss = sessionOpen[0];
                            if (pr_userid.toLowerCase() != ss.OwnerId.toLowerCase()) {
                                $('#txtMessage').prop('disabled', true);
                            }
                            else {
                                $('#txtMessage').prop('disabled', false);
                            }
                            $('#hdfSessionName').val(ss.Name);
                        }
                    }

                    $('#hdfUserId').val(rs.userInfo.UserId);
                    $('#lblChatName').text(rs.userInfo.DisplayName);
                    $('#imgChatDetail').attr('src', rs.userInfo.Avatar);
                    scroll_last();

                    HidePanel();
                    $('#pageChatDetail').show(200);

                }

                $('.iconLoading').hide();
                $('#txtMessage').focus();
            }
        }).done(function () {
            $("#start").click();
            scroll_last();
        });
    } catch (e) { }
}


function formatTime(d, frm) {
    try {
        var time = new Date(d);
        var output = '';
        switch (frm) {
            case 'ymdhm':
                var yy = time.getFullYear();
                var mm = time.getMonth() + 1;
                mm = mm < 10 ? `0${mm}` : mm;
                var dd = time.getDate();
                dd = dd < 10 ? `0${dd}` : dd;

                var hrs = time.getHours() < 10 ? `0${time.getHours()}` : time.getHours();
                var min = time.getMinutes() < 10 ? `0${time.getMinutes()}` : time.getMinutes();
                output = `${yy}-${mm}-${dd} ${hrs}:${min}`

                //output = `${(time.getHours() < 10 ? '0' + time.getHours() : time.getHours())}:${(time.getMinutes() < 10 ? '0' + time.getMinutes() : time.getMinutes())}`
                break;
            case 'ymd':
                var yy = time.getFullYear();
                var mm = time.getMonth() + 1;
                mm = mm < 10 ? `0${mm}` : mm;
                var dd = time.getDate();
                dd = dd < 10 ? `0${dd}` : dd;

                output = `${yy}-${mm}-${dd}`

                //output = `${(time.getHours() < 10 ? '0' + time.getHours() : time.getHours())}:${(time.getMinutes() < 10 ? '0' + time.getMinutes() : time.getMinutes())}`
                break;
            default:
                var hrs = time.getHours() < 10 ? `0${time.getHours()}` : time.getHours();
                var min = time.getMinutes() < 10 ? `0${time.getMinutes()}` : time.getMinutes();
                output = `${hrs}:${min}`
        }

        return output;
    } catch (e) {

    }
}
function draw_chat(rs, isDrawTime) {
    try {
        isDrawTime = isDrawTime != null ? true : false;

        if (rs.data != undefined) {
            open_crm();
            var templateLeft = $('#tmpChatDetailLeft').html();
            var templateRight = $('#tmpChatDetailRight').html();

            var timee = '';

            for (var i = 0; i < rs.data.length; i++) {
                scroll_last();
                var data = rs.data[i];


                var msg = JSON.parse(data.Message); //chỉ có nội dung này là khác nhau

                var html = data.FromId == rs.oaInfo.OAID ? templateRight : templateLeft;

                var avatar = data.FromId == rs.oaInfo.OAID ? rs.oaInfo.Avatar : rs.userInfo.Avatar;

                var msgContent = ``;
                var time = formatTime(data.MsgTime, null);
                //tin nhắn được lấy theo lần đầu tiên
                if (msg.src != undefined) {
                    if (msg.type == 'text') {
                        msgContent = msg.message;
                    }
                    else {
                        msgContent = `${msg.type}`;
                    }
                }

                //tin nhắn được lấy dưới dạng webhook
                else {
                    if (data.Type == 'sticker') {
                        msgContent = `<img width="100" src="${msg.attachments[0].payload.url}" />`;
                    }
                    else {
                        msgContent = msg.text
                    }
                }


                $('#hdfLastMsgId').val(data.MsgId);

                html = html.replace('@avatar', avatar);
                html = html.replace('@time', time);
                html = html.replace('@message', msgContent);

                if (isDrawTime) {

                    var time = formatTime(data.MsgTime, "ymd");
                    if (timee != time) {
                        var tt = `<div class="row"><div class="col-4"><hr /></div><div class="col-4 text-center text-secondary"><small><b>${time}</b></small></div><div class="col-4"><hr /></div></div>`
                        $('#pnlChatDetail').append(tt);
                        timee = time;
                    }
                }

                $('#pnlChatDetail').append(html);
            }

            scroll_last();
        }
    } catch (e) {

    }
}
function draw_chat_session(rs, isPrepend) {
    try {
        isPrepend = isPrepend != null ? true : false;

        if (rs.data != undefined) {
            var templateLeft = $('#tmpChatDetailLeftSession').html();
            var templateRight = $('#tmpChatDetailRightSession').html();

            for (var i = 0; i < rs.data.length; i++) {

                var data = rs.data[i];

                var msg = JSON.parse(data.Message); //chỉ có nội dung này là khác nhau

                var html = data.FromId == rs.oaInfo.OAID ? templateRight : templateLeft;

                var avatar = data.FromId == rs.oaInfo.OAID ? rs.oaInfo.Avatar : rs.userInfo.Avatar;

                var msgContent = ``;
                var time = formatTime(data.MsgTime, null);
                //tin nhắn được lấy theo lần đầu tiên
                if (msg.src != undefined) {
                    if (msg.type == 'text') {
                        msgContent = msg.message;
                    }
                    else {
                        msgContent = `${msg.type}`;
                    }
                }

                //tin nhắn được lấy dưới dạng webhook
                else {
                    if (data.Type == 'sticker') {
                        msgContent = `<img width="100" src="${msg.attachments[0].payload.url}" />`;
                    }
                    else {
                        msgContent = msg.text
                    }
                }
                html = html.replace('@avatar', avatar);
                html = html.replace('@time', time);
                html = html.replace('@message', msgContent);

                if (isPrepend) {
                    $('#pnlChatDetailSession').prepend(html);
                }
                else {
                    $('#pnlChatDetailSession').append(html);
                }
            }
            $('#btnLastPage').click();
        }
    } catch (e) { }
}
function scroll_last() {
    var scrollableDiv = document.getElementById('pnlChatDetail');
    scrollableDiv.scrollTo(0, 99999);
    var scrollableDiv2 = document.getElementById('pnlChatDetailSession');
    scrollableDiv2.scrollTo(0, 99999);
}
$('#txtMessage').bind('keypress', function (e) {
    if (e.keyCode == 13) {
        chat_text();
    }
});
function chat_text() {
    try {

        $.ajax({
            url: api_url + 'chat_text',
            type: "POST",
            data: {
                user_id: $('#hdfUserId').val(),
                text: $('#txtMessage').val()
            },
            success: function (rs) {
                $('#txtMessage').val('');
            }
        });
    } catch (e) { }
}
function chat_sticker(attachment_id) {
    try {

        $.ajax({
            url: api_url + 'chat_sticker',
            type: "POST",
            data: {
                user_id: $('#hdfUserId').val(),
                attachment_id: attachment_id
            },
            success: function (rs) {
                $('.btn-close-sticker').click();
            }
        });
    } catch (e) { }
}

function HidePanel(noAnimation) {
    try {
        noAnimation = noAnimation != undefined ? true : false;
        if (noAnimation) {
            $('#pageContact').hide();
            $('#pageChatList').hide();
            $('#pageChatDetail').hide();
            $('#pageChatDetailSession').hide();
            $('#pageSession').hide();
        }
        else {
            $('#pageContact').hide(200);
            $('#pageChatList').hide(200);
            $('#pageChatDetail').hide(200);
            $('#pageChatDetailSession').hide(200);
            $('#pageSession').hide(200);
        }
    } catch (e) {

    }
}
function RemoveActiveButton() {
    $('.ivg-btn-main').removeClass('active');
}

function contact_list(btn) {
    try {
        RemoveActiveButton();
        $(btn).addClass('active');

        $.ajax({
            url: api_url + 'contact_list',
            type: "GET",
            success: function (rs) {
                console.log(`contact_list`, rs);

                var template = $('#tmpContact').html();

                $('#pnlContact').html('');

                for (var i = 0; i < rs.data.length; i++) {
                    var data = rs.data[i];

                    var html = template.replace('@avatar', data.Avatar);
                    html = html.replace('@name', data.DisplayName);
                    html = html.replace('@user_id', data.UserId);
                    $('#pnlContact').append(html);
                }
                HidePanel();
                $('#pageContact').show(200);


            }
        });
    } catch (e) { }
}

function session_list(btn) {
    try {
        RemoveActiveButton();
        $(btn).addClass('active');
        empty_input();
        $.ajax({
            url: api_url + 'session_list',
            type: "GET",
            data: { userid: pr_userid },
            success: function (rs) {

                console.log(`session_list`, rs);

                $('#tblSessions').html('');

                if (rs.data.length > 0) {
                    var head = ``
                    head += `<tr>`
                    head += `<th>Zalo session Name</th>`
                    head += `</tr>`
                    $('#tblSessionsHead').html(head);
                    for (var i = 0; i < rs.data.length; i++) {
                        var data = rs.data[i];

                        var html = ``;
                        html += `<tr>`;
                        html += `<td onclick="chat_detail_by_session('${data.ID}')">${data.Name}</td>`;
                        html += `</tr>`;
                        html += ``;
                        html += ``;
                        $('#tblSessions').append(html);
                    }
                } else {

                    html = `Không có dữ liệu`;
                    $('#tblSessions').append(html);
                }


                HidePanel();
                $('#pageSession').show(200);
            }
        });
    } catch (e) { }
}

function session_close(isConfirm) {
    try {
        if (!isConfirm) {
            $('#btnCloseSession').click();
        }
        else {
            $.ajax({
                url: api_url + 'session_close',
                type: "GET",
                data: { zaloid: $('#hdfUserId').val() },
                success: function (rs) { 
                    console.log(`session_close`, rs);

                    if (rs.msg != undefined) {
                        $('.btn-close-session').click(); 
                        conversation();
                    }
                }
            });
        }
    } catch (e) { }
}

function chat_detail_by_session(session_id) {
    try {
        $('.iconLoading').show();

        $.ajax({
            url: api_url + 'chat_detail_by_session?session_id=' + session_id,
            type: "GET",
            success: function (rs) {

                console.log(`chat_detail_by_session`, rs);

                $('#pnlChatDetailSession').html('');

                if (rs.data != undefined) {

                    draw_chat_session(rs, 1);

                    //$('#hdfUserId').val(rs.userInfo.UserId);
                    $('#lblChatNameSession').text(rs.userInfo.DisplayName);
                    $('#imgChatDetailSession').attr('src', rs.userInfo.Avatar);
                    scroll_last();

                    HidePanel();
                    $('#pageChatDetailSession').show(200);

                }

                $('.iconLoading').hide();
            }
        }).done(function () {
            scroll_last();
        });
    } catch (e) { }
}

function open_crm() {
    window.parent.postMessage({ type: 'zalo_popup' }, '*');
}
