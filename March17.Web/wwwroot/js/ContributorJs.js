$(() => {
    $("table").on("click", ".btn-success", function () {
        const name = $(this).data("name")
        $("#contributor-name").html(name)
        const id = $(this).data("id")
        $("#deposit-id").prop("value", id)
    })

    $("table").on("click", ".btn-danger", function () {
        $("#first-name").val($(this).data("first-name"))
        $("#last-name").val($(this).data("last-name"))
        $("#cell-number").val($(this).data("cell"))
        $("#date-created").val($(this).data("date"))
        $("#always-include").prop("checked", $(this).data("always-include"))
        $(".deposit").hide()
        $(".form").attr("action", "/contributors/edit")      
    })
    $('.modal-fade').on('hidden.bs.modal', function () {
        $(this).find('form').trigger('reset');
    })

})