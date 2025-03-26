let DDLForChosen = document.getElementById("selectedOptions");
let DLLForAvailable = document.getElementById("availableOptions");


/**Function to switch list item from one DDL to another
 * 
 * @param {any} event
 * @param {any} senderDDL : DDL from which user is multi-selecting
 * @param {any} receiverDDL : DDL that gets the options
 */
function switchOptions(event, senderDDL, receiverDDL) {
    //find all selected options tags - selectedOptions becomes nodelist
    let senderId = senderDDL.id;
    let selectedOption = document.querySelectorAll(`#${senderId} option:checked`);

    event.preventDefault();

    if (selectedOption.length == 0) {
        alert("Nothing to move.")
    }
    else {
        selectedOption.forEach(
            function (o, idx) {
                senderDDL.remove(o.index);
                receiverDDL.appendChild(o);
            }
        );
    }
}

let addOptions = (event) => switchOptions(event, DLLForAvailable, DDLForChosen);
let removeOptions = (event) => switchOptions(event, DDLForChosen, DLLForAvailable);

document.getElementById("btnLeft").addEventListener("click", addOptions);
document.getElementById("btnRight").addEventListener("click", removeOptions);

document.getElementById("btnSubmit").addEventListener("click",
    function () {
        DDlForChosen.childNodes.forEach(opt => opt.selected = "selected");
    }
)