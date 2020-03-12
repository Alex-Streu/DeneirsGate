var NotificationHub = {
    hub: null,

    Init: function () {
        $.connection.hub.start();
        hub = $.connection.notificationHub;

        hub.client.addNotification = function (notification) {
            var total = $('#total-notifications').html();
            var parsed = parseInt(total);
            if (parsed == 'NaN') { parsed = 0; }

            parsed++;
            $('#total-notifications').html(parsed);
            var newItem = `<li>
                                <a href="${notification.Link}">
                                    ${notification.Message}
                                    <span class="small italic">Just now.</span>
                                </a>
                            </li>`;
            $('#notifications-list').prepend(newItem);


            Notiflix.Notify.Info(notification.Message, function (autoClose) {
                if (autoClose) { return true; }

                window.location.href = notification.Link;
            })
        };
    },

    Send: function (username, notification) {
        hub.server.send(username, notification);
    }
}