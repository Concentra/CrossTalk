var config = {
    endpoint: "/api",
    routes: {
        send: "messages",
        list: "messages"
    }
};

var msgTemplate = '<article class="message">\
						<header><span>{from}</span>&nbsp;&nbsp;&#x25B6;&nbsp;&nbsp;<span>{to}</span></header>\
						<img src=""/>\
						<span class="message-content">\
							{Body}\
						</span>\
						<footer>\
							<span class="message-timestamp">{time}</span>\
						</footer>\
					</article>';

$(document).ready(function () {

    function refreshMessages() {
        $('#messages-activity').show();
        $.get(config.endpoint + '/' + config.routes.list + '?callback=?', function (data) {
            $('#messages-activity').hide();
            var result = '';
            console.log(data);
            if (data && data instanceof Array) {
                var row;
                while (row = data.pop()) {
                    result += msgTemplate.replace(/\{[A-Za-z]+\}/gmi, function (match) {
                        return row[match.slice(1, -1)] || match;
                    });
                }
            }
            $('#messages-container').html(result);
        });
    }

    $('input[name="send"]').click(function () {
        $('#send-activity').show();
        $.post(config.endpoint + '/' + config.routes.send, { Edge: "50f82bee76d13f2558d9887f", Body: $('#message').val() }, function () {
            refreshMessages();
            $('#message').val('');
            $('#send-activity').hide();
        });
        return false;
    });

    refreshMessages();
});