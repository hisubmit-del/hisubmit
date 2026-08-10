var GLOBALB = {};
GLOBALB.DotNetReference = [];
GLOBALB.SetDotnetReference = function (pDotNetReference, eId = "1") {
    GLOBALB.DotNetReference.push({ editorId: eId, DotNetReference: pDotNetReference });
};

const toolbarOptions = [
    [{ 'color': [] }, { 'background': [] }],          // dropdown with defaults from theme
    [{ 'font': [] }],
    ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
    ['blockquote', 'code-block'],
    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
    [{ 'script': 'sub' }, { 'script': 'super' }],      // superscript/subscript
    [{ 'indent': '-1' }, { 'indent': '+1' }],          // outdent/indent
    [{ 'direction': 'rtl' }],                         // text direction
    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
    [{ 'align': [] }],
    ['image']
];

const simpleToolbarOptions = [     // dropdown with defaults from theme
    ['bold', 'italic', 'underline'],        // toggled buttons
    ['blockquote'],
    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
    [{ 'direction': 'rtl' }],                         // text direction
    [{ 'align': [] }],
]

let quillList = []
function createRichText(elementId, simpleOption, text = "", enableLink = true) {
    let opts = toolbarOptions;
    if (enableLink)
        toolbarOptions[2].push('link')

    if (simpleOption) {
        if (enableLink)
            simpleToolbarOptions[0].push('link')
        opts = simpleToolbarOptions;
    }
   
    let q = new Quill(`#${elementId}`, {
        theme: 'snow',
        modules: {
            toolbar: opts
        },
    })
    q.pasteHTML(text);

    q.on('text-change', function (delta, oldDelta, source) {
        let content = q.root.innerHTML;
        let rf;
        $.each(GLOBALB.DotNetReference, (index, item) => {
            if (item.editorId === elementId) {
                rf = item.DotNetReference
            }
        })
        rf.invokeMethodAsync('ChangedRicheText', content);
    })
    quillList.push({ editorId: elementId, qu: q });
}

function RemoveRichTextEvent(editorId) {
    let q;
    $.each(quillList, (index, item) => {
        if (item.editorId === editorId) {
            q = item.DotNetReference
        }
    })
    q.off('text-change');
}


let NoUiSlidersList = []
GLOBALB.NoUiSliderReference = [];
GLOBALB.SetNoUiSliderReference = function (pDotNetReference, eId = "1") {
    GLOBALB.NoUiSliderReference.push({ editorId: eId, DotNetReference: pDotNetReference });
};

function CreateNoUiSlider(elementId, step = 5, min = 0, max = 100) {

    let slider = document.getElementById(elementId);

    noUiSlider.create(slider, {
        start: [min, max],
        connect: true,
        range: {
            'min': min,
            'max': max
        },
        step: step,
        margin: 1,
        tooltips: {
            // tooltips are output only, so only a "to" is needed
            to: function (numericValue) {
                return numericValue.toFixed(0);
            }
        },
        // pips: {
        //     mode: 'steps',
        //     density: 5
        // }
    });

    let connect = slider.querySelectorAll('.noUi-connect');
    connect[0].classList.add('bg-red')

    slider.noUiSlider.on('change.one', function () {
        let rf;
        $.each(GLOBALB.NoUiSliderReference, (index, item) => {
            if (item.editorId === elementId) {
                rf = item.DotNetReference
            }
        })
        rf.invokeMethodAsync('ChangeSliderValues', slider.noUiSlider.get(true));
    });
}



