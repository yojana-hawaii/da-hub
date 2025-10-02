$(".tab-content").on("click", ".linkClick", function (event) {
        event.preventDefault(); // on click > dont do default action of going to the page
        var address = $(this).attr("href"); // get the href of the link that was clicked
        var lookup = address.split("/")[1]; // get the controller name 
        var id = $("#" + lookup + "Dropdown").val(); // combine to get dropdownlist identifier

        //nothing selected show h4 tag as caption
        var caption = $("#" + lookup + "Identifier").html();
        if (id == null) {
            alert("Error: no value selected from the " + caption + " list!");
        } else {
            window.location.href = address + "/" + id;
        }
    }
);