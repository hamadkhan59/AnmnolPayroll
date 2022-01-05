$("#search-table").hide();
$("#medicine-search").on("change paste keyup", function () {
    if ($(this).val() == '') {
        $("#search-table").hide();
    }
    else {
        $("#search-table").show();
    }
});

function highlightFirst() {
    $('#search-table tbody tr').removeClass('highlight');
    $("#search-table tbody tr:visible:first").addClass("highlight");
    // $('#search-table tbody tr').filter(':visible').first().addClass("highlight");
}
function highlight(tableIndex) {
    // Just a simple check. If .highlight has reached the last, start again
    if ((tableIndex + 1) > $('#search-table tbody tr').length)
        tableIndex = 0;

    // Element exists?
    if ($('#search-table tbody tr:eq(' + tableIndex + ')').length > 0) {
        // Remove other highlights
        $('#search-table tbody tr').removeClass('highlight');

        // Highlight your target
        $('#search-table tbody tr:eq(' + tableIndex + ')').addClass('highlight');
    }
}

function setTableData(data) {
    debugger;
    var headers = "";
    var rows = "";
    headers += "<tr style='height:30px;'>";
    for (item in data[0]) {
        header += "<th>" + item + "</th>";
    }
    header += "</tr>";
    $("#header").html(header);

    var num_keys = Object.keys(data[0]).length;


    for (item in data) {
        rows += "<tr onclick='placeName(this);'>";
        for (var i = 0; i < num_keys; i++) {
            rows += "<td>" + data[item][Object.keys(data[item])[i]] + "</td>";
        }
        rows += "</tr>";
    }

    $("#table-rows").html(rows);
}


function placeName(row) {
    $("#inputAdmissionNo").val($(row).children("td:nth-child(1)").html());
    $('#search-table tbody tr.highlight').removeClass("highlight");
    $(row).addClass("highlight");

    $("#search-table").hide();
    document.getElementById('fastPayForm').submit();
}


$('#goto_first').click(function () {
    highlight(0);
});

$('#goto_prev').click(function () {
    highlight($('#search-table tbody tr:visible tr.highlight').index() - 1);
});

$('#goto_next').click(function () {
    highlight($('#search-table tbody tr:visible tr.highlight').index() + 1);
});

$('#goto_last').click(function () {
    highlight($('#search-table tbody tr:visible tr:last').index());
});

$(document).keydown(function (e) {

    switch (e.which) {
        case 38:
            index = $('#search-table tbody tr.highlight').index() - 1;
            for (let i = index; i >= 0; i--) {
                if (index >= 0) {
                    if (!$('#search-table tbody tr:eq(' + index + ')').is(":visible"))
                        index--;
                }
                else {
                    index = $('#search-table tbody tr:visible:last').index();
                    break;
                }
            }
            highlight(index < 0 ? $('#search-table tr:visible:last').index() : index);
            break;
        case 40:
            index = $('#search-table tbody tr.highlight').index() + 1;
            for (let i = index; i < $('#search-table tbody tr').length; i++) {
                if (index < $('#search-table tbody tr').length) {
                    if (!$('#search-table tbody tr:eq(' + index + ')').is(":visible"))
                        index++;
                }
                else {
                    index = 0;
                    break
                }
            }
            highlight(index >= $('#search-table tbody tr').length ? $('#search-table tbody tr:visible:first').index() : index);
            break;
        case 13:
            $("#medicine-search").val($('#search-table tbody tr.highlight').children("td:first").html());
            $('#search-table tbody tr.highlight').removeClass("highlight");
            $('#search-table tbody tr:visible:first tr.highlight').addClass("highlight");
            $("#search-table").hide();
            break;
    }


    $("#medicine-search").on("change paste keyup", function (e) {
        if ($(this).val() == '' || e.which == 13) {
            $("#search-table").hide();
        }
        else {
            $("#search-table").show();
        }
        if (e.which != 40 && e.which != 38) {
            highlightFirst();
        }
    });

});


function autocomplete(inp, arr) {
    /*the autocomplete function takes two arguments,
    the text field element and an array of possible autocompleted values:*/
    var currentFocus;
    /*execute a function when someone writes in the text field:*/
    inp.addEventListener("input", function (e) {
        var a, b, i, val = this.value;
        /*close any already open lists of autocompleted values*/
        closeAllLists();
        if (!val) { return false; }
        currentFocus = -1;
        /*create a DIV element that will contain the items (values):*/
        a = document.createElement("DIV");
        a.setAttribute("id", this.id + "autocomplete-list");
        a.setAttribute("class", "autocomplete-items");
        /*append the DIV element as a child of the autocomplete container:*/
        this.parentNode.appendChild(a);
        /*for each item in the array...*/
        for (i = 0; i < arr.length; i++) {
            /*check if the item starts with the same letters as the text field value:*/
            //if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
            /*create a DIV element for each matching element:*/
            if (arr[i].toUpperCase().indexOf(val.toUpperCase()) !== -1) {
                b = document.createElement("DIV");
                b.style.fontSize = "12px";
                /*make the matching letters bold:*/
                b.innerHTML = "<strong>" + arr[i].substr(0, val.length) + "</strong>";
                b.innerHTML += arr[i].substr(val.length);
                /*insert a input field that will hold the current array item's value:*/
                b.innerHTML += "<input type='hidden' value='" + arr[i] + "'>";
                /*execute a function when someone clicks on the item value (DIV element):*/
                b.addEventListener("click", function (e) {
                    /*insert the value for the autocomplete text field:*/
                    inp.value = this.getElementsByTagName("input")[0].value;
                    /*close the list of autocompleted values,
                    (or any other open lists of autocompleted values:*/
                    closeAllLists();
                });
                a.appendChild(b);
            }
        }
    });
    /*execute a function presses a key on the keyboard:*/
    inp.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode == 40) {
            /*If the arrow DOWN key is pressed,
            increase the currentFocus variable:*/
            currentFocus++;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 38) { //up
            /*If the arrow UP key is pressed,
            decrease the currentFocus variable:*/
            currentFocus--;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 13) {
            /*If the ENTER key is pressed, prevent the form from being submitted,*/
            e.preventDefault();
            if (currentFocus > -1) {
                /*and simulate a click on the "active" item:*/
                if (x) x[currentFocus].click();

                GetStopCharges();
            }
        }
    });
    function addActive(x) {
        /*a function to classify an item as "active":*/
        if (!x) return false;
        /*start by removing the "active" class on all items:*/
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        /*add class "autocomplete-active":*/
        x[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(x) {
        /*a function to remove the "active" class from all autocomplete items:*/
        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }
    function closeAllLists(elmnt) {
        /*close all autocomplete lists in the document,
        except the one passed as an argument:*/
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != inp) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }
    /*execute a function when someone clicks in the document:*/
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
        GetStopCharges();
    });
}