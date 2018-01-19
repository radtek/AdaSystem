var linkmanSelect = {};
linkmanSelect.url = "/Customer/LinkMan/GetList";
linkmanSelect.paramsData = function (params) {
    return {
        search: params.term, // search term
        IsBusiness: false
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
    $("#linkname").val(repo.text);
    return repo.text;
};
$.fn.modal.Constructor.prototype.enforceFocus = function () { };

