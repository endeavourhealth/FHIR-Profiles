$(':checkbox').change(function ()
{
    var removedRows = $('.removed-row');

    if ($(this).is(':checked'))
        removedRows.hide();
    else
        removedRows.fadeIn('slow');
});