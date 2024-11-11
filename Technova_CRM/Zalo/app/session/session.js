var crm_user_name = '';


function BindInfo() {

    var queryString = window.location.search;
    var urlParams = new URLSearchParams(queryString);

    var user = urlParams.get('user');
    console.log(user)
    if (user == '' || user == null || user == undefined) {

    }
    else {
        //alert(user);
        crm_user_name = user;
    }
}


function formatTime(d, frm) {
    try {
        var time = new Date(d);
        var output = '';
        switch (frm) {
            case 'ymdhm':
                var yy = time.getFullYear();
                var mm = time.getMonth() - 1;
                mm = mm < 10 ? `0${mm}` : mm;
                var dd = time.getDate();
                dd = dd < 10 ? `0${dd}` : dd;

                var hrs = time.getHours() < 10 ? `0${time.getHours()}` : time.getHours();
                var min = time.getMinutes() < 10 ? `0${time.getMinutes()}` : time.getMinutes();
                output = `${yy}-${mm}-${dd} ${hrs}:${min}`

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
function draw_chat(rs, isPrepend) {
    try {
        isPrepend = isPrepend != null ? true : false;
        if (rs.data != undefined) {
            var templateLeft = $('#tmpChatDetailLeft').html();
            var templateRight = $('#tmpChatDetailRight').html();

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

                if (isPrepend) {
                    if (i == 0) {
                        $('#hdfLastMsgId').val(data.MsgId);
                    }
                }
                else {
                    $('#hdfLastMsgId').val(data.MsgId);
                }


                html = html.replace('@avatar', avatar);
                html = html.replace('@time', time);
                html = html.replace('@message', msgContent);
                if (isPrepend) {
                    $('#pnlChatDetail').prepend(html);
                }
                else {
                    $('#pnlChatDetail').append(html);

                }

            }


        }
    } catch (e) {

    }
}
function scroll_last() {
    var scrollableDiv = document.getElementById('pnlChatDetail');
    scrollableDiv.scrollTo(10000, 10000);
}

function HidePanel() {

    $('#pageContact').hide(200);
    $('#pageChatList').hide(200);
    $('#pageChatDetail').hide(200);
    $('#pageSession').hide(200);
}
function contact_list(btn) {
    try {

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


session_list();
function session_list() {
    try {
        var queryString = window.location.search;
        var urlParams = new URLSearchParams(queryString);

        var zaloid = urlParams.get('zaloid');

        $.ajax({
            url: api_url + 'session_list?zaloid=' + zaloid,
            type: "GET",
            success: function (rs) {
                console.log(`session_list`, rs);


                $('#tblSessions').html('');

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

                if (rs.data.length == 0) {
                    var html = ``;
                    html += `<tr>`;
                    html += `<td colspan="9999">Không có dữ liệu</td>`;
                    html += `</tr>`;
                    $('#tblSessions').html(html);

                }
                HidePanel();
                $('#pageSession').show(200);
            }
        });
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

                $('#pnlChatDetail').html('');

                if (rs.data != undefined) {

                    draw_chat(rs, 1);

                    $('#hdfUserId').val(rs.userInfo.UserId);
                    $('#lblChatName').text(rs.userInfo.DisplayName);
                    $('#imgChatDetail').attr('src', rs.userInfo.Avatar);
                    scroll_last();

                    HidePanel();
                    $('#pageChatDetail').show(200);

                }
                if (rs.sessionInfo != undefined) {
                    var ss = rs.sessionInfo;
                    var tmp = $('#tmpSessionDetail').html();

                    $('#pnlSessionDetail').html('');

                    $('#pnlSessionDetail').append(tmp.replace('@name', 'Zalo Session Name').replace('@value', ss.Name));
                    $('#pnlSessionDetail').append(tmp.replace('@name', 'Start Time').replace('@value', ss.StartTime != null ? formatTime(ss.StartTime, 'ymdhm') : '-'));
                    $('#pnlSessionDetail').append(tmp.replace('@name', 'Response Time').replace('@value', ss.ResponseTime != null ? formatTime(ss.ResponseTime, 'ymdhm') : '-'));
                    $('#pnlSessionDetail').append(tmp.replace('@name', 'End Time').replace('@value', ss.EndTime != null ? formatTime(ss.EndTime, 'ymdhm') : '-'));
                    $('#pnlSessionDetail').append(tmp.replace('@name', 'Status').replace('@value', ss.StatusText));
                    $('#pnlSessionDetail').append(tmp.replace('@name', 'Owner').replace('@value', ss.OwnerName));
                    $('#pnlSessionDetail').append(tmp.replace('@name', 'Zalo User').replace('@value', ss.ZaloUserName));
                }
                scroll_last();

                $('.iconLoading').hide();
            }
        }).done(function () {
            scroll_last();
        });
    } catch (e) { }
}