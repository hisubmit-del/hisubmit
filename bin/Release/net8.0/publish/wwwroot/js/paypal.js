var GLOBAL = {};
GLOBAL.DotNetReference = null;
GLOBAL.SetDotnetReferencePaypal = function (pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
};
function RenderPaypalButton(container, onApprovedFunctionName, amount, costumeId) {
    if (typeof paypal === 'undefined') {
        return false;
    }

    const containerElement = typeof container === 'string'
        ? document.querySelector(container)
        : container;

    if (!containerElement) {
        console.error("Container not found:", container);
        return false;
    }

    // ✅ پاک کردن محتوای قبلی (دکمه قبلی PayPal)
    containerElement.innerHTML = '';

    paypal.Buttons({
        style: {
            shape: 'pill',
            color: 'gold',
            label: 'paypal'
        },

        createOrder: (data, actions) => {
            return actions.order.create({
                purchase_units: [{
                    amount: {
                        value: amount
                    },
                    description: "description for order",
                    custom_id: costumeId
                }]
            });
        },

        onApprove: (data, actions) => {
            return actions.order.capture().then((detail) => {
                let d = {
                    payerId: data.payerID,
                    paymentId: data.paymentID,
                    email: detail.payer.email_address,
                    orderId: detail.id,
                    price: parseFloat(detail.purchase_units[0].amount.value),
                    cartId: parseInt(detail.purchase_units[0].custom_id)
                };

                if (GLOBAL.DotNetReference) {
                    GLOBAL.DotNetReference.invokeMethodAsync(onApprovedFunctionName, d);
                } else {
                    console.error("DotNet reference not set.");
                }
            });
        }

    }).render(containerElement);

    return true;
}

function RemoveButtonContainer(container) {
    const containerElement = typeof container === 'string'
        ? document.querySelector(container)
        : container;

    if (containerElement) {
        containerElement.innerHTML = '';
  
    }
}

//function RenderPaypalButton
//(container, onApprovedFunctionName, amount, costumeId) {
//    if (typeof paypal === 'undefined') {
//        return false;
//    } else {
//        paypal.Buttons({
//            style: {
//                shape: 'pill',
//                color: 'gold',
//                label: 'paypal'
//            },

//            createOrder: (data, actions) => {
//                return actions.order.create({
//                    purchase_units: [{
//                        amount: {
//                            value: amount
//                        },
//                        description: "description for order",
//                        custom_id: costumeId
//                    }]
//                })
//            },

//            //createSubscription: (data, actions) => {
//            //    return actions.subscription.create({
//            //        'plan-id': planeId
//            //    })
//            //},

//            onApprove: (data, actions) => {
//                return actions.order.capture().then((detail) => {                  
//                    let cId = detail.id;
//                    let payerId = data.payerID;
//                    let payingId = data.paymentID
//                    let emailAddress = detail.payer.email_address;
//                    let d = {
//                        payerId: payerId,
//                        paymentId: payingId,
//                        email: emailAddress,
//                        orderId: cId,
//                        price:parseFloat(detail.purchase_units[0].amount.value),
//                        cartId:parseInt(detail.purchase_units[0].custom_id)
//                    }
//                    GLOBAL.DotNetReference.invokeMethodAsync(onApprovedFunctionName, d);
//                    //DotNet.invokeMethodAsync(onApprovedFunctionAssembly,                                                                                                                              
//                    //    onApprovedFunctionName, data, actions);
//                })
//            }
//        }).render(container);
//        return true;
//    }
//}
