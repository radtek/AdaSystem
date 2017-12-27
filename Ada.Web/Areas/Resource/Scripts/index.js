function groupDetail(id) {
    $("#modalView").load("/Resource/MediaGroup/Detail/" + id,
        function () {
            $('#modalView .modal').on('shown.bs.modal', function () {
                $('[data-toggle="tooltip"]').tooltip();
            }).on('hidden.bs.modal', function () {

            });
            $('#modalView .modal').modal('show');
        });

}
function formatterPrice(value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<span class='label label-info'>" + v.AdPositionName + "：" + (v.PurchasePrice == 0 ? "不接单" : v.PurchasePrice) + "　　有效期至：" + (v.InvalidDate ? moment(v.InvalidDate).format("YYYY-MM-DD") : "") + "</span><br/>";
        });
    return result;
}

function formatterGroup(value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<button class='btn btn-warning btn-xs' onclick=\"groupDetail('" + v.Id + "');\"><i class='fa fa-object-group'></i> " + v.GroupName + "</button> ";
        });
    return result;
}