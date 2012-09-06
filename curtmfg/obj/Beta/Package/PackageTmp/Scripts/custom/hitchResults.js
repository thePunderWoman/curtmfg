var end = "";

$(function () {

	Shadowbox.init({
		height: '80%',
		width: '80%',
		enableKeys: false
	});

	$('#fromName').live('blur', function () {
		if ($.trim($(this).val()) != '') {
			var text = $(this).val() + end;
			$('#messagePreview').html(text);
		}
	});

    $('.mail').toggle();
    $('.mail img').toggle();

});

function mailPart(prodID) {
    var message = "[Your name] would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    end = " would like to share a CURT Part with you. http://www.curtmfg.com/part/" + prodID;
    $('#messagePreview').text(message);
    var html = $('#shadowbox_mail').html();
    Shadowbox.open({
        content: html,
        player: 'html',
        width: 600,
        height: 475
    });
}