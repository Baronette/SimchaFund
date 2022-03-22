$(() => {
    $('#checkbox').change(function () {
        $(this).closest('tr').find('amount').prop('value', "5")
    })
});