var transactorSelect = {};
transactorSelect.url = "/Admin/Manager/GetList";
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
    $("#Transactor").val(repo.text);
    return repo.text;
};
$(function () {
    initSelect("TransactorId", transactorSelect);
});

