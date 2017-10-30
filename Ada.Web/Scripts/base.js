//删除
function del(id, value, url) {
    swal({
        title: "您确定吗?",
        text: "您确定要删除 " + value + "?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false
    }, function () {
        var magicToken = $("input[name=__RequestVerificationToken]").first();
        if (!magicToken) { return; }
        var form = $("<form action=\"" + url + "\" method=\"POST\" />");
        form.append(magicToken.clone());
        form.append($("<input type=\"hidden\" name=\"id\" value=\"" + id + "\" />"));
        $("body").append(form);
        form.submit();
    });
}
//新增
function add() {
    $('#modal form')[0].reset();
    $("#Id").val("");
    $(".modal-title span").text("添加");
    $('#modal').modal('show');
}
//编辑
function update(id, url) {
    $.getJSON(url, { id: id },
        function (data) {
            $('#modal form').autofill(data);
            $(".modal-title span").text("编辑");
            $('#modal').modal('show');
        });
}