var fansSelect = {};
fansSelect.url = "/Tools/Fans/GetList";
fansSelect.paramsData = function (params) {
    return {
        search: params.term
    };
};
fansSelect.processResults = function (data, params) {
    var result = $.map(data.rows,
        function (v, k) {
            return { id: v.Id, text: v.NickName };
        });
    return {
        results: result
    };
};
fansSelect.formatRepo = function (repo) {
    if (repo.loading) {
        return repo.text;
    }
    return repo.text;
};
fansSelect.formatRepoSelection = function (repo) {
    return repo.text;
};
$(function () {
    initSelect2("ParentId", fansSelect);
});
function initSelect2(id, opt) {
    $("#" + id).select2({
        placeholder: "请输入关键字",
        language: "zh-CN",
        ajax: {
            url: opt.url,
            dataType: 'json',
            delay: 250,
            data: opt.paramsData,
            processResults: opt.processResults,
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // 字符转义处理
        minimumInputLength: 1,
        templateResult: opt.formatRepo, //返回结果回调function formatRepo(repo){return repo.text},这样就可以将返回结果的的text显示到下拉框里，当然你可以return repo.text+"1";等
        templateSelection: opt.formatRepoSelection //选中项回调function formatRepoSelection(repo) { return repo.text }

    });
}
