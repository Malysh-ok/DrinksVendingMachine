// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#senderBtn").click(function () {
        
        // Получаем все значения всех монет
        let purchaseParts = [];
        $("#purchasePartial DIV").each(function () {
            let item = {};
            item.CoinId = +$(this).find(            // + вначале - преобразуем в число
                ".requestDataKey").val();
            item.CoinCount = +$(this).find(         // + вначале - преобразуем в число
                ".requestDataValue").val();
            purchaseParts.push(item);
        });
        
        // Получаем Id напитка
        let drinkId = $("#Drinks").val();
        
        // Объект, для отправки контроллеру
        let dataFromView = {}
        dataFromView.DrinkId = drinkId;
        dataFromView.PurchaseParts = purchaseParts;

        // Запрос контроллеру
        $.ajax({
            url: 'User/Edit',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(dataFromView),

            success: function (partialView) {
                $("#Drinks").prop('selectedIndex', 0);  // сбрасываем дропбокс
                $('#purchasePartial').html(partialView);            // заполняем монеты
            },
            
            error: function (msg) {
                console.log("!!!!! ERROR !!!!!\n", msg);
            }
        });
    })
});
