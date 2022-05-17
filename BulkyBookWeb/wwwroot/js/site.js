// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function(){

    
    $('[data-toggle="popover"]').popover({
        content: function () {
            return $("#notification-content").html();
            //it will return all the contents of the element that has the id notification-content
            //and display to the body of popover
        },
        html:true    //so the popover can parse and display html contents
    });

    //create new element notification-content and append it to the body
    $('body').append(`<div id="notification-content" hidden></div>`)

    function getNotification() {
        //here we are extracting notifications from GetNotification method in Notification controller
        //and appending text of each notification to <ul> list
        //and also extracting count and adding it to badge in navbar

        var res = "<ul  class='list-group'>";

        $.ajax({
            url: "/Admin/Notification/GetNotification",
            method: "GET",
            success: function (result) {
                if (result.count != 0) {
                    $("#notificationCount").html(result.count);
                }
                else {
                    $("#notificationCount").html();
                    $("#notificationCount").hide('slow');
                    $("#notificationCount").popover('hide');
                }
                var notifications = result.notificationApplicationUser;
                notifications.forEach(element => {
                    /*console.log(element.notification.text);*/
                    //console.log(element.notification.id);

                    res = res + "<li id='" + element.notification.id + "' class='list-group-item notification-text'>" + element.notification.text + "</li>";
                });
                
                res = res + "</ul>";

                //here we are appending this list to "notification-content" in order to display it in popover
                $("#notification-content").html(res);

                console.log(result);
            },
            error: function (error) {
                console.log(error);
            }
        });
    }


    //since the popover list has been generated during runtime we need to go to parent of element
    //from which we want to capture the event
    $(document).on('click', 'li.notification-text', function (e) {
        
        var target = e.target;
        console.log(target);
        var id = $(target).attr('id');
        console.log(id);

        readNotification(id, target);
    });
    
    function readNotification(id, target) {
        $.ajax({
            url: "/Admin/Notification/ReadNotification",
            method: 'GET',
            data: {
                notificationId:id
            },
            success: function (result) {
                getNotification();
                $(target).fadeOut('slow');
            },
            error: function (error) {
                console.log(error);
            }
        })
    }

    getNotification();

    //connectiong to NotificationHub
    var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();

    connection.on('displayNotification', function () {
        getNotification();
    });

    connection.start();
});