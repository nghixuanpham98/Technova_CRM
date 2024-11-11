

BindData();

var liParamsLink = [];
function BindData() {
    try {
        $('.iconLoading').show();
        var settings = {
            url: api_url + 'zns_template_list',
            type: "GET",
        }
        $.ajax(settings).done(function (rs) {
            console.log(rs); 
            $('.iconLoading').hide();
            if (rs != undefined) {
                if (rs.data != undefined) {
                    $('#thead').html('');
                    $('#tbody').html('');

                    if (rs.columns != undefined) {
                        var html = ``;
                        html += `<tr>`;
                        for (var i = 0; i < rs.columns.length; i++) {
                            html += `<th>${rs.columns[i]}</th>`;
                        }
                        html += `</tr>`;
                        $('#thead').append(html);
                    }

                    for (var i = 0; i < rs.data.length; i++) {
                        var html = ``;
                        var onclick = `onclick="Detail('${rs.data[i].ID}')"`
                        html += `<tr ${onclick}>`;
                        for (var j = 0; j < rs.columns.length; j++) {
                            html += `<td style="word-wrap:break-word">${rs.data[i][rs.columns[j]]}</td>`;
                        }
                        html += `</tr>`;
                        $('#tbody').append(html);
                    }
                }
                if (rs.dataParamsLink != undefined) {
                    liParamsLink = [];
                    liParamsLink.push({ entity: '---', field: '---' });
                    for (var i = 0; i < rs.dataParamsLink.length; i++) {
                        liParamsLink.push({
                            entity: rs.dataParamsLink[i].EntityName,
                            field: rs.dataParamsLink[i].FieldName,
                        });
                    }
                }

            }
        });

    } catch (e) { }
}

BindOAInfo();

function BindOAInfo() {
    try {
        $('.iconLoading').show();
        var settings = {
            url: api_url + 'oa_info',
            type: "GET",
        }
        $.ajax(settings).done(function (rs) {
            if (rs != undefined) {

                //{
                //    "OAID": "980920582523369584",
                //    "Name": "IVG JSC",
                //    "Description": "Công ty Cổ Phần IVG Việt nam là nhà cung cấp tiên phong trong lĩnh vực chăm sóc khách hàng và quản trị kinh doanh. Với đội ngũ chuyên gia tài năng giàu kinh nghiệm, được hỗ trợ toàn diện bởi mạng lưới đối tác trên toàn cầu, IVG luôn sẵn sàng tư vấn, cung cấp, triển khai và khai thác hiệu quả các hệ thống được tối ưu hóa cao độ, đáp ứng những yêu cầu cầu đặc thù và phù hợp với định hướng phát triển của từng tổ chức, doanh nghiệp.",
                //    "IsVerified": true,
                //    "OAType": 2,
                //    "CateName": "Công nghệ & Thiết bị",
                //    "NumFollower": 14,
                //    "Avatar": "https://s160-ava-talk.zadn.vn/a/2/4/6/3/160/fd8330fcc4e59743c87d3aa6947cba7c.jpg",
                //    "Cover": "https://cover-talk.zadn.vn/f/2/9/3/4/fd8330fcc4e59743c87d3aa6947cba7c.jpg",
                //    "PackageName": "Nâng cao",
                //    "PackageValidThroughDate": "18/03/2025",
                //    "PackageAutoRenewDate": "19/03/2025",
                //    "LinkedZca": "ZBA-134092",
                //    "RemainingQuotaPromotion": 0,
                //    "RemainingQuota": 4999,
                //    "DailyQuotaPromotion": 0,
                //    "DailyQuota": 5000,
                //    "CreatedOn": "2024-10-04T13:41:25.153",
                //    "ModifiedOn": "2024-10-22T12:49:39.7"
                //}

            }
            console.log(`BindOAInfo`, rs);
        });

    } catch (e) { }
}
function NewParamsLink(entity, field) {
    try {
        entity = entity != undefined ? entity : '';
        field = field != undefined ? field : '';
        var html = ``;
        html += `<tr style="background-color:antiquewhite">`;
        html += `<td></td>`;
        html += `<td><input class="form-control txtEntityName" spellcheck="false" autocomplete="off" value="${entity}" /></td>`;
        html += `<td><input class="form-control txtFieldName" spellcheck="false" autocomplete="off" value="${field}" /></td>`;
        html += `</tr>`;
        $('#tbodyParamsLink').append(html);

    } catch (e) {

    }

}
function SaveParamsLink() {
    try {
        $('.iconLoading').show(); 
        var ListParamLink = []; 
        $('#tbodyParamsLink tr').each(function () { 
            ListParamLink.push({
                EntityName: $(this).find('.txtEntityName').val(),
                FieldName: $(this).find('.txtFieldName').val()
            });
        });

        console.log(ListParamLink);

        var settings = {
            url: api_url + 'zns_params_save',
            type: "POST",
            data: { ListParamLink: ListParamLink }
        }
        $.ajax(settings).done(function (rs) {
            console.log(rs)
            $('.iconLoading').hide();

            if (rs != undefined && rs.error != undefined) {

            }
            else {
                BindParams();
            }

        });

    } catch (e) { }
}


function DeleteParamsLink(entity, field) {
    try {
        var cf = confirm('Bạn có chắc chắn muốn xóa tham số?');
        if (cf) {

            $('.iconLoading').show();

            var ListParamLink = [];

            ListParamLink.push({
                EntityName: entity,
                FieldName: field
            });

            var settings = {
                url: api_url + 'zns_params_delete',
                type: "POST",
                data: { ListParamLink: ListParamLink }
            }
            $.ajax(settings).done(function (rs) {
                console.log(rs)
                $('.iconLoading').hide();
                if (rs != undefined && rs.error != undefined) {

                }
                else {
                    BindParams();
                }

            });
        }

    } catch (e) { }
}


function BindParams() {
    try {
        $('.iconLoading').show();
        var settings = {
            url: api_url + 'zns_params_list',
            type: "GET",
        }
        $.ajax(settings).done(function (rs) {
            console.log(rs)
            $('.iconLoading').hide();
        
            if (rs != undefined && rs.data != undefined) {

                var thead = ``;
                thead += `<tr>`;
                thead += `<th></th>`;
                thead += `<th>Entity Name</th>`;
                thead += `<th>Field Name</th>`;
                thead += `</tr>`;
                $('#theadParamsLink').html(thead);

                $('#tbodyParamsLink').html('');

                for (var i = 0; i < rs.data.length; i++) {

                    var btn = ``;
                    btn += `<div class="ivg-btn">`;
                    btn += `<button onclick="NewParamsLink('${rs.data[i].EntityName}','${rs.data[i].FieldName}')"><i class="fas fa-copy"></i></button>`;
                    btn += `<button onclick="DeleteParamsLink('${rs.data[i].EntityName}','${rs.data[i].FieldName}')"><i class="fas fa-times"></i></button>`;
                    btn += `</div>`;
          
                    var html = ``;
                    html += `<tr>`;
                    html += `<td>${btn}</td>`; 
                    html += `<td><input disabled class="form-control txtEntityName" spellcheck="false" autocomplete="off" value="${rs.data[i].EntityName}" /></td>`;
                    html += `<td><input disabled class="form-control txtFieldName" spellcheck="false" autocomplete="off" value="${rs.data[i].FieldName}" /></td>`;
                    html += `</tr>`;
                    $('#tbodyParamsLink').append(html);
                }
            }
        });

    } catch (e) { }
}


function Detail(templateId) {
    try {
        $('.iconLoading').show();
        var settings = {
            url: api_url + 'zns_template_detail?TemplateID=' + templateId,
            type: "GET",
        }
        $.ajax(settings).done(function (rs) {
            console.log(rs)
            $('.iconLoading').hide();
            if (rs != undefined) {

                if (rs.dataDetail != undefined && rs.dataDetail.length == 1) {

                    var detail = rs.dataDetail[0];

                    $('#txtTemplateName').val(detail.TemplateName);
                    $('#txtStatus').val(detail.Status);
                    $('#txtTimeout').val(detail.Timeout);
                    $('#txtTemplateQuality').val(detail.TemplateQuality);
                    $('#txtTemplateTag').val(detail.TemplateTag);
                    $('#txtPrice').val(detail.Price);
                    $('#txtApplyTemplateQuota').val(detail.ApplyTemplateQuota);
                    $('#hdfTemplateID').val(detail.TemplateID);
                    $('#txtPreviewUrl').attr('href', detail.PreviewUrl);
                    $('#frmPreviewUrl').attr('src', detail.PreviewUrl);
                    $('#txtPreviewUrl').show();

                    if (rs.dataParams != undefined && rs.dataParams.length > 0) {

                        var tableName = 'Params';

                        if (rs.dataParams != undefined) {
                            $('#thead' + tableName).html('');
                            $('#tbody' + tableName).html('');

                            var thead = ``;
                            thead += `<tr>`;
                            thead += `<th>Name</th>`;
                            thead += `<th>Type</th>`;
                            thead += `<th>Max length</th>`;
                            thead += `<th>Min length</th>`;
                            thead += `<th>Link with CRM</th>`;
                            thead += `</tr>`;
                            $('#thead' + tableName).html(thead);

                            for (var i = 0; i < rs.dataParams.length; i++) {

                                var ddlLink = `<select class="form-control ddlCRMEntities select2">`;

                                for (var j = 0; j < liParamsLink.length; j++) {
                                    var entity = liParamsLink[j].entity;
                                    var field = liParamsLink[j].field;
                                    var selected = rs.dataParams[i].LinkWithEntity == entity && rs.dataParams[i].LinkWithField == field ? 'selected' : '';
                                    var option = `<option ${selected} data-entity="${entity}" data-field="${field}">${entity} - ${field}</option>`;
                                    ddlLink += option;
                                }
                                ddlLink += `</select>`;
                                var hdfId = `<input class="hdfID" hidden value="${rs.dataParams[i].ID}" />`;
                                var html = ``;
                                html += `<tr>`;
                                html += `<td>${hdfId}<input disabled class="form-control txtName" spellcheck="false" autocomplete="off" value="${rs.dataParams[i].Name}" /></td>`;
                                html += `<td><input disabled class="form-control txtType" spellcheck="false" autocomplete="off" value="${rs.dataParams[i].Type}" /></td>`;
                                html += `<td><input disabled class="form-control txtMaxLength" spellcheck="false" autocomplete="off" value="${rs.dataParams[i].MaxLength}" /></td>`;
                                html += `<td><input disabled class="form-control txtMinLength" spellcheck="false" autocomplete="off" value="${rs.dataParams[i].MinLength}" /></td>`;
                                html += `<td style="word-wrap:break-word">${ddlLink}</td>`;

                                html += `</tr>`;
                                $('#tbody' + tableName).append(html); 
                            }
                            $('.ddlCRMEntities').css('width','100%');
                            $('.ddlCRMEntities').select2();
                        } 
                    } 
                }
            }
        });

    } catch (e) { }
}

function SaveParams() {
    try {
        $('.iconLoading').show();
        var ListParams = [];
        $('#tbodyParams tr').each(function () {

            var entity = $(this).find('.ddlCRMEntities option:selected').attr('data-entity');
            var field = $(this).find('.ddlCRMEntities option:selected').attr('data-field');

            ListParams.push({
                ID: $(this).find('.hdfID').val(),
                Name: $(this).find('.txtName').val(),
                Type: $(this).find('.txtType').val(),
                MaxLength: $(this).find('.txtMaxLength').val(),
                MinLength: $(this).find('.txtMinLength').val(),
                FieldName: $(this).find('.txtFieldName').val(),
                LinkWithEntity: entity,
                LinkWithField: field,
            });
        });

        var settings = {
            url: api_url + 'zns_template_save',
            type: "POST",
            data: { ListParams: ListParams }
        }
        $.ajax(settings).done(function (rs) {
            console.log(rs);
            $('.iconLoading').hide();

            if (rs != undefined && rs.error != undefined) {

            }
            else {
                BindParams();
            }

        });

    } catch (e) { }
}

function draw_table(rs, tableName) {
    tableName = tableName != undefined ? tableName : '';

    if (rs.data != undefined) {
        $('#thead' + tableName).html('');
        $('#tbody' + tableName).html('');

        if (rs.columns != undefined) {
            var html = ``;
            html += `<tr>`;
            for (var i = 0; i < rs.columns.length; i++) {
                html += `<th>${rs.columns[i]}</th>`;
            }
            html += `</tr>`;
            $('#thead' + tableName).append(html);
        }

        for (var i = 0; i < rs.data.length; i++) {
            var html = ``;
            html += `<tr>`;
            for (var j = 0; j < rs.columns.length; j++) {
                html += `<td style="word-wrap:break-word">${rs.data[i][rs.columns[j]]}</td>`;
            }
            html += `</tr>`;
            $('#tbody' + tableName).append(html);
        }
    }
}



function BindUsers() {
    try {
        $('.iconLoading').show();
        var settings = {
            url: api_url + 'user_list',
            type: "GET",
        }
        $.ajax(settings).done(function (rs) {
            console.log(rs);
            $('.iconLoading').hide();
            if (rs != undefined && rs.data != undefined) {


                if (rs.columns != undefined) {
                    var html = ``;
                    html += `<tr>`;
                    for (var i = 0; i < rs.columns.length; i++) {
                        html += `<th>${rs.columns[i]}</th>`;
                    }
                    html += `</tr>`;
                    $('#theadUsers').append(html);
                }

                var thead = ``;
                thead += `<tr>`;
                thead += `<th></th>`;
                thead += `<th>Name</th>`;
                thead += `<th>Quan tâm</th>`;
                thead += `<th>Thao tác</th>`;
                thead += `</tr>`;
                $('#theadUsers').html(thead);

                $('#tbodyUsers').html('');

                for (var i = 0; i < rs.data.length; i++) {

                    var isFollower = rs.data[i].IsFollower == true ? `<span class="badge rounded-pill bg-success opacity-50">Đã quan tâm</span>`
                        : '<span class="badge rounded-pill bg-light text-dark opacity-50">Chưa quan tâm</span>';

                    var sentRequest = rs.data[i].LastRequestInfoOn != null ? 'disabled' : '';
                    var sendZNS = rs.data[i].Phone != null ? '' : 'disabled';

                    var btnInteraction = `<div class="ivg-btn">`
                        + `<button ${sentRequest} onclick="request_share_info('${rs.data[i].UserId}')" data-bs-toggle="tooltip" data-bs-placement="top" title="Gửi yêu cầu chia sẻ thông tin"><i class="fa-solid fa-share-nodes"></i></button>`
                        + `&nbsp;&nbsp;<button ${sendZNS} onclick="zns_template_send('${rs.data[i].UserId}')" data-bs-toggle="tooltip" data-bs-placement="top" title="Gửi ZNS"><i class="fa-solid fa-paper-plane"></i></button>`
                        + `&nbsp;&nbsp;<button ${sendZNS} data-bs-toggle="tooltip" data-bs-placement="top" title="Nhắn tin"><i class="fa-solid fa-comments"></i></button>`
                        + `</div>`;

                    btnInteraction = rs.data[i].Phone != null ? btnInteraction : '';

                    var html = ``;
                    var onclick = ``// `onclick="Detail('${rs.data[i].ID}')"`
                    html += `<tr ${onclick}>`;
                    html += `<td valign="middle" class="text-center"><input type="checkbox" class="ivg-ckb ck-row" data-ava="${rs.data[i].Avatar}" data-name="${rs.data[i].DisplayName}" data-id="${rs.data[i].UserId}" onchange="row_checked()" /></td>`;
                    html += `<td valign="middle"><img src="${rs.data[i].Avatar}" width="40" class="ivg-user-ava" /> ${rs.data[i].DisplayName}</td>`;
                    html += `<td valign="middle">${isFollower}</td>`;
                    html += `<td valign="middle">${btnInteraction}</td>`; 
                    html += `</tr>`;
                    $('#tbodyUsers').append(html);

                } 
            }
        });

    } catch (e) { }
}
 

function row_checked() {
    $('#tbodyUsers tr').each(function () {
        var tr = $(this)[0];
        var ck = $(tr).find('.ck-row').prop('checked')
        if (ck == true) {
            $(tr).addClass('ivg-row-checked')
        }
        else {
            $(tr).removeClass('ivg-row-checked')
        }
    });

}




function bind_user_list_for_share() {
    $('#tbodyUserChecked').html('');
    $('.ck-row').each(function () {
        var checked = $(this).prop('checked');
        var id = $(this).attr('data-id');
        var name = $(this).attr('data-name');
        var ava = $(this).attr('data-ava');
        if (checked) {
            var html = ``;
            html += `<tr>`;
            html += `<td>`;
            html += `<button onclick="remove_user_list_for_share('${id}')" class="btn btn-outline-danger btn-sm"><i class="fas fa-times"></i></button>`;
            html += `<img src="${ava}" width="40" class="ivg-user-ava" /> ${name}`;

            html += `</td>`;
            html += `</tr>`;
            html += ``;
            html += ``;
            $('#tbodyUserChecked').append(html);
        }

        $('#lblTotalUserChecked').text($('#tbodyUserChecked tr').length);
    });
    row_checked();

}

function remove_user_list_for_share(remove_id) {
    if (remove_id != undefined) {
        $('.ck-row').each(function () {
            var id = $(this).attr('data-id');
            if (remove_id == id) {
                $(this).prop('checked', false);
            }
        });
        bind_user_list_for_share();
    }
    else {

    }

}

function request_share_info(user_id) {
    try {
        //hỏi xác nhận gửi template
        var cf = confirm('Bạn có chắc chắn muốn gửi yêu cầu chia sẻ thông tin từ người dùng?');

        if (cf) {
            $('.iconLoading').show();

            var list_user_id = [];
            $('.ck-row').each(function () {
                if ($(this).prop('checked') == true) {
                    list_user_id.push($(this).attr('data-id'));
                }
            });

            var settings = {
                url: api_url + 'request_share_info',
                type: "POST",
                data: {
                    user_id: user_id
                }
            }

            $.ajax(settings).done(function (rs) {
                console.log(rs);
                $('.iconLoading').hide();
                if (rs != undefined && rs.error != undefined) {

                }
                else {
                    $('.btn-close').click();
                }
            });
        }
    } catch (e) { }
}
function zns_template_send(user_id) {
    try {
        var cf = confirm('Bạn có chắc chắn muốn gửi ZNS đến danh sách người dùng đã chọn?');
        if (cf) { 
            $('.iconLoading').show();
            var settings = {
                url: api_url + 'zns_template_send',
                type: "POST",
                data: {
                    user_id: user_id
                }
            }
            $.ajax(settings).done(function (rs) {
                console.log(rs);
                $('.iconLoading').hide();
                if (rs != undefined && rs.data != undefined) {

                }
            });
        } 
    } catch (e) { }
}
 
$(".ivg-btn button").mouseover(function () {
    var i = $(this).find('i')[0];
    $(i).addClass('fa-bounce');
});

$(".ivg-btn button").mouseout(function () {
    var i = $(this).find('i')[0];
    $(i).removeClass('fa-bounce');
});
