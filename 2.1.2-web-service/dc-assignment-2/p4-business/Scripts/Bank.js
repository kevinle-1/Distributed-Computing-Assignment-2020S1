//
// Filename: bank.js
// Project:  DC Assignment (COMP3008)
// Purpose:  Functionality for User/ Account/ Transaction pages on Web view. 
// Author:   Kevin Le (19472960)
//
// Note:     Javascript inline brackets - Not Curtin guideline
//
// Date:     24/05/2020
//

//Users

// Purpose: Get user details based on user ID input 
function GetUserDetails() {
    $.ajax(
        {
            url: "/api/Bank/UserDetails/" + $(".UserDetails #id").val(),
            type: "get",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr) {
                console.log(data);
                var obj = JSON.parse(data); //Convert to JSON as recieved as string 

                if (obj.fName != null) { //Empty object -> No result returned 
                    $(".UserDetails #fName").text(obj.fName);
                    $(".UserDetails #lName").text(obj.lName);
                }
                else {
                    alert("Error: User not found");
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                console.log(errorThrown);
                alert("Error: Could not retrieve user details")
            }
        });
}

// Purpose: Create new user based on fName and lName input 
function CreateUser() {
    $.ajax(
        {
            url: "/api/Bank/CreateUser/" + $(".CreateUser #fName").val() + "/" + $(".CreateUser #lName").val(),
            type: "post",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr) {
                console.log(textStatus);
            },
            error: function (jqXhr, textStatus, errorThrown) {
                console.log(errorThrown);
                alert("Error: Could not create user")
            }
        });
}

// Purpose: Update existing users First and Last name based on User ID 
function UpdateUserName() {
    $.ajax(
        {
            url: "/api/Bank/UpdateUser/" + $(".UpdateUser #id").val() + "/" + $(".UpdateUser #fName").val() + "/" + $(".UpdateUser #lName").val(),
            type: "post",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr) {
                console.log(data);
                console.log(textStatus);

                if (data == "false") {
                    alert("Error: Could not update user (Do they exist?)");
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                console.log(errorThrown);
                alert("Error: Could not update user");
            }
        });
}

//Accounts

// Purpose: Create new account based on User ID provided
function CreateAccount() {
    $.ajax(
        {
            url: "/api/Bank/CreateAccount/" + $(".CreateAccount #id").val(),
            type: "post",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr) {

            },
            error: function (jqXhr, textStatus, errorThrown) {
                console.log(errorThrown);
                alert("Error: Could not create account");
            }
        });
}

// Purpose: Get account details based on Account ID provided
function GetAccountDetails() {
    $.ajax(
        {
            url: "/api/Bank/AccountDetails/" + $(".AccountDetails #id").val(),
            type: "get",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr)
            {
                console.log(data);
                var obj = JSON.parse(data); //Convert to JSON as recieved as string 

                if (obj.accId != 0)
                {
                    $(".AccountDetails #uid").text(obj.userId);
                    $(".AccountDetails #bal").text(obj.bal);
                }
                else
                {
                    alert("Error: Account does not exist");
                }

            },
            error: function (jqXhr, textStatus, errorThrown)
            {
                console.log(errorThrown);
                alert("Error: Could not retrieve account details");
            }
        });
}

// Purpose: Deposit amount into account based on account ID and amount textbox
function Deposit() {
    $.ajax(
        {
            url: "/api/Bank/Deposit/" + $(".AccountTransaction #id").val() + "/" + $(".AccountTransaction #amount").val(),
            type: "post",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr) {
                if (data == "false") {
                    alert("Error: Could not perform deposit (Does account exist?)");
                }
            },
            error: function (jqXhr, textStatus, errorThrown) {
                console.log(errorThrown);
                alert("Error: Could not perform deposit");
            }
        });
}

// Purpose: Withdraw amount from account based on account ID and amount textbox
function Withdraw() {
    $.ajax(
    {
        url: "/api/Bank/Withdraw/" + $(".AccountTransaction #id").val() + "/" + $(".AccountTransaction #amount").val(),
        type: "post",
        contentType: "application/json",
        processData: false,
        success: function (data, textStatus, jQxhr) {
            if (data == "false") {
                alert("Error: Could not perform withdrawal (Does account exist?)");
            }
        },
        error: function (jqXhr, textStatus, errorThrown) {
            console.log(errorThrown);
            alert("Error: Could not perform withdrawal");
        }
    });
}

//Transaction 

// Purpose: Send money between accounts given account ID from, account ID to and amount
function Send() {
    $.ajax(
        {
            url: "/api/Bank/Send/" + $(".TransactionSend #senderId").val() + "/" + $(".TransactionSend #recieveId").val() + "/" + $(".TransactionSend #amount").val(),
            type: "post",
            contentType: "application/json",
            processData: false,
            success: function (data, textStatus, jQxhr) {
                if (data == "false") {
                    alert("Error: Could not perform transaction (Are account IDs correct?");
                }
            }
        });
}

// Purpose: Get details of transaction based on transaction ID (If still in queue)
function GetTransactionDetails() {
    $.ajax(
    {
        url: "/api/Bank/Transaction/" + $(".TransactionDetails #id").val(),
        type: "get",
        contentType: "application/json",
        processData: false,
        success: function (data, textStatus, jQxhr) {
            console.log(data);
            var obj = JSON.parse(data); //Convert to JSON as recieved as string 

            if (obj.senderId != 0)
            {
                $(".TransactionDetails #from").text(obj.senderId);
                $(".TransactionDetails #to").text(obj.recieverId);
                $(".TransactionDetails #amount").text(obj.amount);
            }
            else {
                alert("Error: Could not retrieve transaction details");
            }
        },
        error: function (jqXhr, textStatus, errorThrown) {
            console.log(errorThrown);
            alert("Error: Could not retrieve transaction details");
        }
    });
}