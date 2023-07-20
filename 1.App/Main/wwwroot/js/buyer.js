// Для страницы Buyer

// Массив идентификаторов "запланированных вызовов" (setTimeout)
let timeoutStack = [];

// Сброс дропбокса с напитками, цены и всех значений монет
function clearMainModelData() {
    $("#Drinks").prop("selectedIndex", 0);
    $("#DrinkPrice").val(null);
    $("#Coins DIV").each(function () {
        $(this).find("[data-requestDataValue]").val(0);
    })
}

// Получение объекта, связанного с покупкой, для отправки контроллеру
function getPurchaseData() {
    // Получаем все значения всех монет
    let purchaseParts = [];
    $("#Coins div").each(function () {
        let item = {};
        item.CoinId = +$(this).find(            // + вначале - преобразуем в число
            ".requestDataKey").val();
        item.CoinCount = +$(this).find(         // + вначале - преобразуем в число
            ".requestDataValue").val();
        purchaseParts.push(item);
    });

    // Получаем Id напитка
    let drinkId = +$("#Drinks").val();

    // Объект, для отправки контроллеру
    let dataFromView = {}
    dataFromView.DrinkId = drinkId;
    dataFromView.PurchaseParts = purchaseParts;
    dataFromView.IsBuyerGetSubjects = isBuyerGetSubjects();

    return dataFromView;
}

// Признак того, что покупатель забрал сдачу и напиток
function isBuyerGetSubjects() {
    return  !$('#ChangeFromPurchase').val() && !$('#BoughtDrink').val();
}

// Обрабатываем ответ сервера после нажатия кнопок в Представлении
function handleAjaxResponse(response){
    
    if (response.redirectUrl != null) {
        // Если пришел ответ о перенаправлении
        window.location.href = response.redirectUrl;
    }
    
    if (response.isClearMainModelData) {
        // Очищаем напитки, монеты
        clearMainModelData();
    }
    
    if (response.infoHtml != null) {
        // "Пишем" в Информацию
        let elem = $('#Info');
        elem.html(response.infoHtml);
        let timeout = +elem.find($("[data-clear]")).attr("data-clear") || 0;
        // dataClearVal.match(/(?<=\s+|^)content(?=\s+|$)/i)
        if(timeout > 0) {
            timeoutStack.forEach(item => clearTimeout(item));   // отмена всех "запланированных вызовов"
            timeoutStack.push(setTimeout(function () {
                    elem.html(null)
                }, timeout
            ));
        }
    }
    
    if (response.change != null) {
        // "Пишем" в Сдачу
        $('#ChangeFromPurchase').val(response.change);
    }

    if (response.drink != null) {
        // "Пишем" в Напиток
        $('#BoughtDrink').val(response.drink);
    }
}

// Обрабатываем ответ сервера если ошибка
function handleErrorAjaxResponse(response) {
    
    let infoHtml = "<span class='text-danger small'>HTTP ERROR " + response.status + ". " 
        // + response.responseText + "</span>";
        + "Подробную информацию см. в консоли Инструментов разработчика."
        + "<br/>Для дальнейшей работы рекомендуем перезагрузить страницу.</span>"
    $('#Info').html(infoHtml);
}

$(document).ready(function () {

    // Смена напитка - меняем отображаемую цену
    $("#Drinks").change(function () {
        $("#DrinkPrice").val(+$("#Drinks option:selected").attr("data-price"));
    })
    
    // Кнопка "Купить"
    $("#BuyBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Buyer/Buy',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(getPurchaseData()),

            beforeSend: function (request) {
                request.setRequestHeader("RequestVerificationToken", $("[name='__RequestVerificationToken']").val());
            },

            success: function (response) {
                handleAjaxResponse(response)
            },
            
            error: function (response) {
                console.log("!!!!! ERROR !!!!!\n", response);
                handleErrorAjaxResponse(response);
            }
        })
    })

    // Кнопка "Отмена"
    $("#EscBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Buyer/Escape',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(getPurchaseData()),

            beforeSend: function (request) {
                request.setRequestHeader("RequestVerificationToken", $("[name='__RequestVerificationToken']").val());
            },

            success: function (response) {
                handleAjaxResponse(response)
            },

            error: function (response) {
                console.log("!!!!! ERROR !!!!!\n", response);
                handleErrorAjaxResponse(response);
            }
        })
    })

    // Кнопка "Забрать"
    $("#GetBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Buyer/UserReceiving',
            type: 'POST',
            contentType: "application/json; charset=utf-8",

            beforeSend: function (request) {
                request.setRequestHeader("RequestVerificationToken", $("[name='__RequestVerificationToken']").val());
            },

            success: function (response) {
                handleAjaxResponse(response);
            },

            error: function (response) {
                console.log("!!!!! ERROR !!!!!\n", response);
                handleErrorAjaxResponse(response);
            }
        })
    })
})
