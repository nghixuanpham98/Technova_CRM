
setInterval(heartbeat, 1000);

chat();
function chat() {
    try {
        var queryString = window.location.search;
        var urlParams = new URLSearchParams(queryString);

        var userid = urlParams.get('userid');
        if (userid == '' || userid == null || userid == undefined) {
            chat_list();
        }
        else {
            chat_detail(userid);
        }
    } catch (e) {

    }
}
function formatTime(d,frm) {
    try {
        var time = new Date(d);
        var output = '';
        switch (frm) {

            default: output = `${(time.getHours() < 10 ? '0' + time.getHours() : time.getHours())}:${(time.getMinutes() < 10 ? '0' + time.getMinutes() : time.getMinutes())}`
        }
        var hrs = time.getHours() < 10 ? `0${time.getHours()}` : time.getHours();
        var min = time.getMinutes() < 10 ? `0${time.getMinutes()}` : time.getMinutes();
        output = `${hrs}:${min}`
        return output;
    } catch (e) {

    }
}

function chat_list() {
    try {
        $.ajax({
            url: api_url + 'chat_list',
            type: "GET",
            success: function (rs) {

                console.log(`chat_list`, rs);

                empty_input();

                if (rs.data != undefined) {

                    var template = $('#tmpChatList').html();

                    $('#pnlChatList').html('');

                    for (var i = 0; i < rs.data.length; i++) {
                        var data = rs.data[i];

                        var avatar = data.from_id == zalo_oa_id ? data.to_avatar : data.from_avatar;
                        var name = data.from_id == zalo_oa_id ? data.to_display_name : data.from_display_name;
                        var user_id = data.from_id == zalo_oa_id ? data.to_id : data.from_id;

                        var html = template.replace('@avatar', avatar);
                        html = html.replace('@name', name);
                        html = html.replace('@user_id', user_id);
                        html = html.replace('@message', data.type == "text" ? data.message : `[${data.type}]`);
                        $('#pnlChatList').append(html);
                    }
                    $('#pageChatList').show(200);
                    $('#pageChatDetail').hide(200);
                }
            }
        }).done(function () {
        });
    } catch (e) { }
}
$('#txtMessage').change(function () {
    chat_text();
})
function empty_input() {
    $('#hdfUserId').val('');
}

function chat_detail(user_id) {
    try {
        user_id = user_id != undefined ? user_id : $('hdfUserId').val();

        $.ajax({
            url: api_url + 'chat_detail?user_id=' + user_id,
            type: "GET",
            success: function (rs) {

                console.log(`chat_detail`, rs);

                $('#pnlChatDetail').html('');

                if (rs.data != undefined) {

                    draw_chat(rs, 1);

                    $('#hdfUserId').val(rs.userInfo.UserId);
                    $('#lblChatName').text(rs.userInfo.DisplayName);
                    $('#imgChatDetail').attr('src', rs.userInfo.Avatar); 

                    $('#pageChatList').hide(200);
                    $('#pageChatDetail').show(200);

                    var scrollableDiv = document.getElementById('pnlChatDetail');
                    scrollableDiv.scrollTo(0, 10000);
                }
            }
        });
    } catch (e) { }
}
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
                draw_chat(rs); 
                //console.log(`chat_detail_recall`, rs);
            }
        });
    } catch (e) { }
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
            var scrollableDiv = document.getElementById('pnlChatDetail');
            scrollableDiv.scrollTo(0, 10000);

        }
    } catch (e) {

    }
}
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



sticker();
function sticker() {
    try {

        $.ajax({
            url: api_url + 'sticker',
            type: "GET",
            success: function (rs) {
                console.log(`sticker`, rs);
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
        var input = qa_get_info_save_db();

        console.log(`qa_save`, input);

        var frmData = new FormData();

        frmData.append('QAInput', JSON.stringify(input));

        $.ajax({
            url: api_uat_claim + 'qa_save',
            type: "POST",
            data: frmData,
            cache: false,
            contentType: false,
            processData: false,
            success: function (rs) {
                console.log(`qa_save rs`, rs)
            }
        });

    } catch (e) {

    }
}



