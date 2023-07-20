// Для страницы Admin

// Массив идентификаторов "запланированных вызовов" (setTimeout)
let timeoutStack = [];

// Очистка настройки напитков
function clearDrinks() {
    $("#Drinks").prop("selectedIndex", 0);
    // $("#Drinks option:eq('')").prop("selected", true)        // или так
    $("#DrinkPrice").val(null);
    $("#DrinkCount").val(null);
}

// Очистка добавления напитка
function clearAdditionDrink() {
    $("#AddDrinkName").val(null);
    $("#AddDrinkPrice").val(null);
    $("#AddDrinkCount").val(null);
}

// Очистка настройки монет
function clearCoins() {
    $("#Coins").prop("selectedIndex", 0);
    // $("#Coins option:eq('')").prop("selected", true)        // или так
    $("#CoinCount").val(null);
    $("#CoinIsLocked").prop("checked", false);
}

// Получение объекта, связанного с настройкой напитка, для отправки контроллеру
function getDrink() {
    // Объект, для отправки контроллеру
    let drink = {}
    drink.Id = +$("#Drinks").val();
    drink.Count = +$("#DrinkCount").val();
    drink.Price = +$("#DrinkPrice").val();

    return drink;
}

// Получение объекта, связанного с добавлением/удалением напитка, для отправки контроллеру
function getAddedDrink() {
    // Объект, для отправки контроллеру
    let drink = {}
    drink.Name = $("#AddDrinkName").val();
    drink.Count = +$("#AddDrinkCount").val();
    drink.Price = +$("#AddDrinkPrice").val();

    return drink;
}

// Получение объекта, связанного с настройкой монет, для отправки контроллеру
function getCoin() {
    // Объект, для отправки контроллеру
    let coin = {}
    coin.Id = +$("#Coins").val();
    coin.Count = +$("#CoinCount").val();
    coin.IsLocked = $("#CoinIsLocked").prop("checked");

    return coin;
}

// Обрабатываем ответ сервера после нажатия кнопок в Представлении
function handleAjaxResponse(response){
    
    if (response.redirectUrl != null) {
        // Если пришел ответ о перенаправлении
        window.location.href = response.redirectUrl;
    }
    
    if (response.isClearDrinks) {
        // Очищаем настройку напитков
        clearDrinks();
    }

    if (response.isClearAdditionDrink) {
        // Очищаем добавление напитка
        clearAdditionDrink();
    }

    if (response.isClearCoins) {
        // Очищаем монеты
        clearCoins();
    }

    if (response.drinksHtml != null) {
        // "Пишем" в Настройки напитка
        let drinks = $("#Drinks");
        let drinksIndex = drinks.prop("selectedIndex");     // копируем индекс выбранного option
        drinks.html(response.drinksHtml)
        drinks.prop("selectedIndex", drinksIndex);                      // восстанавливаем индекс выбранного option
        let drinkOptSel = $("#Drinks option:selected");
        $("#DrinkPrice").val(+drinkOptSel.attr("data-price"));
        $("#DrinkCount").val(+drinkOptSel.attr("data-count"));
    }

    if (response.coinsHtml) {
        // "Пишем" в Настройки монет
        let coins = $("#Coins");
        let coinsIndex = coins.prop("selectedIndex");     // копируем индекс выбранного option
        coins.html(response.coinsHtml)
        coins.prop("selectedIndex", coinsIndex);                      // восстанавливаем индекс выбранного option
        let coinsOptSel = $("#Coins option:selected");
        $("#CoinCount").val(+coinsOptSel.attr("data-count"));
        $("#CoinIsLocked").prop('checked', !!+coinsOptSel.attr("data-isLocked"));
    }

    if (response.infoHtml != null) {
        // "Пишем" в Информацию
        let elem = $('#Info');
        elem.html(response.infoHtml);
        let timeout = +elem.find($("[data-clear]")).attr("data-clear") || 0;
        if(timeout > 0) {
            timeoutStack.forEach(item => clearTimeout(item));   // отмена всех "запланированных вызовов"
            timeoutStack.push(setTimeout(function () {
                    elem.html(null)
                }, timeout
            ));
        }
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
        let drinkOptSel = $("#Drinks option:selected");
        $("#DrinkPrice").val(+drinkOptSel.attr("data-price"));
        $("#DrinkCount").val(+drinkOptSel.attr("data-count"));
    })

    // Смена монеты - меняем количество и признак блокировки
    $("#Coins").change(function () {
        let coinsOptSel = $("#Coins option:selected");
        $("#CoinCount").val(+coinsOptSel.attr("data-count"));
        $("#CoinIsLocked").prop('checked',
            !!+coinsOptSel.attr("data-isLocked"));
    })

    // Кнопка "Применить" (для Напитка)
    $("#ApplyDrinkBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Admin/ApplyDrink',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(getDrink()),

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

    // Кнопка "Добавить" (для Напитка)
    $("#AddDrinkBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Admin/AddDrink',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(getAddedDrink()),

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

    // Кнопка "Удалить" (для Напитка)
    $("#RemoveDrinkBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Admin/RemoveDrink',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(getAddedDrink()),

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

    // Кнопка "Применить" (для Монет)
    $("#ApplyCoinBtn").click(function () {
        // Запрос контроллеру
        $.ajax({
            url: 'Admin/ApplyCoin',
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(getCoin()),

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
            url: 'Admin/Escape',
            type: 'POST',
            contentType: "application/json; charset=utf-8",

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
})