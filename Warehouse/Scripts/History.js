function SubmitSearch(searchParameter) {
    var searchByParameter = document.getElementById("searchByParameter").value;
    
    if (searchParameter != null && searchParameter != '') {
        location.href = '../Content/History.aspx?' + searchByParameter +'=' + searchParameter;
    }
    else {
        alert("Please enter a search parameter!");
    }
};