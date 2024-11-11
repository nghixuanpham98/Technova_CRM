BindData();
function BindData() {
    try {
        var settings = {
            url: api_url + 'query_table',
            type: "POST",
            data: {
                tableName: $('#ddlTableName option:selected').val(),
                tableRows: $('#ddlTableRows option:selected').val(),
            }
        }
        $.ajax(settings).done(function (rs) { console.log(rs); draw_table(rs); });

    } catch (e) { }
}

function draw_table(rs) {

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
            html += `<tr>`;
            for (var j = 0; j < rs.columns.length; j++) {
                //html += `<td><textarea class="form-control" style="background:unset;border:unset;">${rs.data[i][rs.columns[j]]}</textarea></td>`;
                html += `<td style="word-wrap:break-word">${rs.data[i][rs.columns[j]]}</td>`;
            }
            html += `</tr>`;
            $('#tbody').append(html);
        }
    }
}

