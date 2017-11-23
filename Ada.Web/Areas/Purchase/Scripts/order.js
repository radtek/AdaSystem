var linkmanSelect = {},
    transactorSelect = {};
linkmanSelect.url = linkmanUrl;
linkmanSelect.paramsData = function (params) {
    return {
        search: params.term, // search term
        IsBusiness: true
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
    $("#LinkManName").val(repo.text);
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
    $("#Transactor").val(repo.text);
    return repo.text;
};
$(function () {
    initData();
    $(".wrapper.wrapper-content form").validate({
        submitHandler: function (form) {
            $('.wrapper.wrapper-content .ibox').children('.ibox-content').toggleClass('sk-loading');
            form.submit();
        }
    });
});
//初始化
function initData() {
    $('.input-group.date').datetimepicker({
        language: 'zh-CN',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        format: "yyyy年mm月dd日"
    });
    initSelect2("LinkManId", linkmanSelect);
    initSelect2("TransactorId", transactorSelect);
    //计算金额
    var taxinput = $('#Tax'),
        discountinput = $('#DiscountMoney'),
        bargininput = $('#BargainMoney'),
        purchaseinput = $('#PurchaseMoney'),
        taxmoneyinput = $('#TaxMoney'),
        totalinput = $('#Money');
    taxinput.add(discountinput).add(bargininput).add(purchaseinput).add(totalinput).on('input propertychange', function () {
        var tax = taxinput.val() || 0,
            discount = discountinput.val() || 0,
            bargin = bargininput.val() || 0,
            purchase = purchaseinput.val() || 0,
            money = totalinput.val() || 0,
            taxmoney = taxmoneyinput.val() || 0,
            currentid = $(this).attr("id"),
            currentvalue = $(this).val();
        if (currentid == "PurchaseMoney" && !isNaN(currentvalue)) {//无税金额
            money = Math.toFixMoney(currentvalue * (1 + tax / 100)) - discount;
            taxmoney = Math.toFixMoney(currentvalue * (tax / 100));
            taxmoneyinput.val(taxmoney);
            totalinput.val(money);
            moneyBefore = money;
            purchaseBefore = currentvalue;
        }
        if (currentid == "Money" && !isNaN(currentvalue)) {//采购金额
            purchase = Math.toFixMoney(currentvalue / (1 + tax / 100)) - discount;
            taxmoney = Math.toFixMoney(purchase * (tax / 100));
            taxmoneyinput.val(taxmoney);
            purchaseinput.val(purchase);
            moneyBefore = currentvalue;
            purchaseBefore = purchase;
        }
        //if (currentid == "BargainMoney" && !isNaN(currentvalue)) {//定金
        //    money = Math.toFixMoney(money*1 - discount*1 - currentvalue*1);
        //    purchase = Math.toFixMoney(purchase*1 - discount*1 - currentvalue*1);
        //    taxmoney = Math.toFixMoney(purchase * (tax / 100));
        //    taxmoneyinput.val(taxmoney);
        //    totalinput.val(money);
        //    purchaseinput.val(purchase);
        //}
        if (currentid == "DiscountMoney" && !isNaN(currentvalue)) {//优惠金额
            money = Math.toFixMoney(moneyBefore*1 - currentvalue*1);
            purchase = Math.toFixMoney(purchaseBefore*1 - currentvalue*1);
            taxmoney = Math.toFixMoney(purchase * (tax / 100));
            taxmoneyinput.val(taxmoney);
            totalinput.val(money);
            purchaseinput.val(purchase);
        }
        if (currentid == "Tax" && !isNaN(currentvalue)) {//税率
            if (!isNaN(purchase)) {
                money = Math.toFixMoney(purchase * (1 + currentvalue / 100) - discount * 1);
                totalinput.val(money);
            } else if (!isNaN(money)) {
                purchase = Math.toFixMoney(money / (1 + currentvalue / 100) - discount * 1);
                purchaseinput.val(purchase);
            }
            moneyBefore = money;
            purchaseBefore = purchase;
            taxmoney = Math.toFixMoney(purchase * (currentvalue / 100));
            taxmoneyinput.val(taxmoney);
            
        }
     
    });
}

//初始化下拉查找控件
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


