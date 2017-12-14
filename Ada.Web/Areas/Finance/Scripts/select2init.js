var linkmanSelect = {},
    transactorSelect = {};

linkmanSelect.url = linkmanUrl;
linkmanSelect.paramsData = function (params) {
    return {
        search: params.term // search term
    };
};
linkmanSelect.processResults = function (data, params) {
    var result = $.map(data.rows,
        function (v, k) {
            return { id: v.Id, text: v.Name, commpany: v.CommpanyName };
        });
    return {
        results: result
    };
};
linkmanSelect.formatRepo = function (repo) {
    if (repo.loading) {
        return repo.text;
    }
    return "<p>" + repo.commpany + " 【 " + repo.text + " 】 <p>";
};
linkmanSelect.formatRepoSelection = function (repo) {
    $linkmanVal.val(repo.text);
    return repo.text;
};

transactorSelect.url = managerUrl;
transactorSelect.paramsData = function (params) {
    return {
        search: params.term, // search term
        Status: 1
    };
};
transactorSelect.processResults = function (data, params) {
    var result = $.map(data.rows,
        function (v, k) {
            return { id: v.Id, text: v.UserName };
        });
    return {
        results: result
    };
};
transactorSelect.formatRepo = function (repo) {
    if (repo.loading) {
        return repo.text;
    }
    return repo.text;
};
transactorSelect.formatRepoSelection = function (repo) {
    $managerVal.val(repo.text);
    return repo.text;
};
$(function () {
    initSelect2($linkman, linkmanSelect);
    initSelect2($manager, transactorSelect);
});
function initSelect2(obj, opt) {
    obj.select2({
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