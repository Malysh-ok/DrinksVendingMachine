// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#senderBtn").click(function () {
        let requestData = [];
        $("#purchasePartial DIV").each(function () {
            let item = {};
            item.CoinId = +$(this).find(            // + вначале - преобразуем в число
                ".requestDataKey").val();
            item.CoinCount = +$(this).find(         // + вначале - преобразуем в число
                ".requestDataValue").val();
            requestData.push(item);
        });

        $.ajax({
            url: 'User/Edit',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(requestData),

            success: function (partialView) {
                $('#purchasePartial').html(partialView);
                // $('#purchasePartial').show();
            },
            
            error: function (msg) {
                console.log("!!!!! ERROR !!!!!\n", msg);
            }
        });
    })
});
