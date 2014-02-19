function onBeforeClientDelete(record) {
    if (confirm("Are you sure you want to delete this record? \n\n" + "\nItem Code: " + record.ITEMCODE + "\n\Kit Code: " + record.KITCODE + "\nAlternate Item Name: " + record.ALTERNATETEXT) == false) {
        return false;
    }
    return true;
}

function validate(record) {
    for (var i in record) {
      
        if (i != 'KITITEMID' && record[i] == ''){
                alert("Column " + i + " is mandatory.");
                return false;
        }
    }
    return true;
}
