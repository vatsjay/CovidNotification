// A custom function to set cookies
function setCookie(name, value, daysToLive) {
    const cookie = name + "=" + encodeURIComponent(value);
    if (typeof daysToLive === "number") {
        cookie += "; max-age=" + (daysToLive * 24 * 60 * 60);
        document.cookie = cookie;
    }
};
// A custom function to get cookies
function getCookie(name) {
    var cookieArr = document.cookie.split(";");
    for (var i = 0; i < cookieArr.length; i++) {
        var cookiePair = cookieArr[i].split("=");
        if (name == cookiePair[0].trim()) {
            return decodeURIComponent(cookiePair[1]);
        }
    }
    // Return null if not found
    return null;
}
// Notification Functions
const PushNotifications = (function () {
    let applicationServerPublicKey;

    let districtName;
    let consoleOutput;
    let pushServiceWorkerRegistration;
    let subscribeButton, unsubscribeButton;
    let topicInput, urgencySelect, notificationInput;

    function initializeConsole() {
        districtName = document.getElementById('district');
        consoleOutput = document.getElementById('output');
        document.getElementById('clear').addEventListener('click', clearConsole);
    }

    function clearConsole() {
        while (consoleOutput.childNodes.length > 0) {
            consoleOutput.removeChild(consoleOutput.lastChild);
        }
    }

    function writeToConsole(text) {
        var paragraph = document.createElement('p');
        paragraph.style.wordWrap = 'break-word';
        paragraph.appendChild(document.createTextNode(text));

        consoleOutput.appendChild(paragraph);
    }

    function messageStrip(textMsg, type) {
        const msgContainer = document.getElementById('msgStrip');
        msgContainer.style.display = 'block';
        if (type === 'error') {
            msgContainer.classList.remove('success');
            msgContainer.classList.add('error');
        }
        if (type === 'success') {
            msgContainer.classList.remove('error');
            msgContainer.classList.add('success');
            closePopup();
        }
        const msgText = document.getElementById('msgText');
        msgText.innerHTML = textMsg;

        setTimeout(function () {
            msgContainer.style.display = 'none'
        }, 5000);
    };


    function registerPushServiceWorker() {
        navigator.serviceWorker.register('/scripts/service-workers/push-service-worker.js', { scope: '/scripts/service-workers/push-service-worker/' })
            .then(function (serviceWorkerRegistration) {
                pushServiceWorkerRegistration = serviceWorkerRegistration;

                initializeUIState();

                writeToConsole('Push Service Worker has been registered successfully');
            }).catch(function (error) {
                writeToConsole('Push Service Worker registration has failed: ' + error);
            });
    }

    function initializeUIState() {
        subscribeButton = document.getElementById('subscribe');
        subscribeButton.addEventListener('click', subscribeForPushNotifications);

        unsubscribeButton = document.getElementById('unsubscribe');
        unsubscribeButton.addEventListener('click', unsubscribeFromPushNotifications);

        topicInput = document.getElementById('topic');
        notificationInput = document.getElementById('notification');
        urgencySelect = document.getElementById('urgency');
        document.getElementById('send').addEventListener('click', sendPushNotification);

        pushServiceWorkerRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                changeUIState(Notification.permission === 'denied', subscription !== null);
            });
    }

    function changeUIState(notificationsBlocked, isSubscibed) {
        subscribeButton.disabled = notificationsBlocked || isSubscibed;
        unsubscribeButton.disabled = notificationsBlocked || !isSubscibed;

        if (notificationsBlocked) {
            writeToConsole('Permission for Push Notifications has been denied');
            messageStrip('Permission for Push Notifications has been denied', 'error')
        }
    }

    function subscribeForPushNotifications() {
        if (applicationServerPublicKey) {
            subscribeForPushNotificationsInternal();
        } else {
            PushNotificationsController.retrievePublicKey()
                .then(function (retrievedPublicKey) {
                    applicationServerPublicKey = retrievedPublicKey;
                    writeToConsole('Successfully retrieved Public Key');

                    subscribeForPushNotificationsInternal();
                }).catch(function (error) {
                    writeToConsole('Failed to retrieve Public Key: ' + error);
                });
        }
    }

    function subscribeForPushNotificationsInternal() {
        pushServiceWorkerRegistration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: applicationServerPublicKey
        })
            .then(function (pushSubscription) {
                PushNotificationsController.storePushSubscription(pushSubscription, districtName.value)
                    .then(function (response) {
                        if (response.ok) {
                            setCookie("subscribed", true, 8589589);
                            const successMsg = `Successfully subscribed for Push Notifications for ${districtName.value}`
                            writeToConsole('Successfully subscribed for Push Notifications');
                            messageStrip(successMsg, 'success');
                        } else {
                            writeToConsole('Failed to store the Push Notifications subscrition on server');
                            messageStrip('Failed to store the Push Notifications subscrition', 'error')
                        }
                    }).catch(function (error) {
                        writeToConsole('Failed to store the Push Notifications subscrition on server: ' + error);
                    });

                changeUIState(false, true);
            }).catch(function (error) {
                if (Notification.permission === 'denied') {
                    changeUIState(true, false);
                } else {
                    writeToConsole('Failed to subscribe for Push Notifications: ' + error);
                }
            });
    }

    function unsubscribeFromPushNotifications() {
        pushServiceWorkerRegistration.pushManager.getSubscription()
            .then(function (pushSubscription) {
                if (pushSubscription) {
                    pushSubscription.unsubscribe()
                        .then(function () {
                            PushNotificationsController.discardPushSubscription(pushSubscription)
                                .then(function (response) {
                                    if (response.ok) {
                                        setCookie("subscribed", '', 0);
                                        writeToConsole('Successfully unsubscribed from Push Notifications');
                                        messageStrip('Successfully unsubscribed from Push Notifications', 'error')
                                    } else {
                                        writeToConsole('Failed to discard the Push Notifications subscrition from server');
                                        messageStrip('Failed to discard the Push Notifications subscrition from server', 'error')
                                    }
                                }).catch(function (error) {
                                    writeToConsole('Failed to discard the Push Notifications subscrition from server: ' + error);
                                });

                            changeUIState(false, false);
                        }).catch(function (error) {
                            writeToConsole('Failed to unsubscribe from Push Notifications: ' + error);
                        });
                }
            });
    }

    function sendPushNotification() {
        let payload = { topic: topicInput.value, notification: notificationInput.value, urgency: urgencySelect.value };

        fetch('push-notifications-api/notifications', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                if (response.ok) {
                    writeToConsole('Successfully sent Push Notification');
                } else {
                    writeToConsole('Failed to send Push Notification');
                }
            }).catch(function (error) {
                writeToConsole('Failed to send Push Notification: ' + error);
            });
    }

    return {
        initialize: function () {
            initializeConsole();

            if (!('serviceWorker' in navigator)) {
                writeToConsole('Service Workers are not supported');
                messageStrip('Service Workers are not supported', 'error')
                return;
            }

            if (!('PushManager' in window)) {
                writeToConsole('Push API not supported');
                return;
            }

            registerPushServiceWorker();
        }
    };
})();

PushNotifications.initialize();
