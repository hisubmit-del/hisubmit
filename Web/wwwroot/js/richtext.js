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
    // Build a fresh toolbar array so repeated editors do not mutate the
    // shared toolbar and accidentally re-enable links in later editors.
    let opts = enableLink
        ? toolbarOptions.map((group, index) => index === 2
            ? [...group, 'link']
            : (Array.isArray(group) ? [...group] : group))
        : toolbarOptions
            .filter(group => !(Array.isArray(group) && group.includes('link')))
            .map(group => Array.isArray(group) ? [...group] : group);

    if (simpleOption) {
        opts = enableLink
            ? simpleToolbarOptions.map((group, index) => index === 0
                ? [...group, 'link']
                : (Array.isArray(group) ? [...group] : group))
            : simpleToolbarOptions
                .filter(group => !(Array.isArray(group) && group.includes('link')))
                .map(group => Array.isArray(group) ? [...group] : group);
    }
   
    let q = new Quill(`#${elementId}`, {
        theme: 'snow',
        modules: {
            toolbar: opts
        },
    })
    q.pasteHTML(enableLink ? text : stripRichTextLinks(text));

    q.on('text-change', function (delta, oldDelta, source) {
        let content = enableLink ? q.root.innerHTML : stripRichTextLinks(q.root.innerHTML);
        if (!enableLink && content !== q.root.innerHTML) {
            const selection = q.getSelection();
            q.clipboard.dangerouslyPasteHTML(content, 'silent');
            if (selection) q.setSelection(selection);
        }
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

function stripRichTextLinks(html) {
    const template = document.createElement('template');
    template.innerHTML = html || '';
    template.content.querySelectorAll('a, script, style, iframe, object, embed, form').forEach(node => {
        if (node.tagName.toLowerCase() === 'a') {
            node.replaceWith(...Array.from(node.childNodes));
        } else {
            node.remove();
        }
    });
    template.content.querySelectorAll('*').forEach(node => {
        Array.from(node.attributes).forEach(attribute => {
            const name = attribute.name.toLowerCase();
            const value = attribute.value.toLowerCase();
            if (name.startsWith('on') || name === 'href' || name === 'src' ||
                value.includes('javascript:') || value.includes('data:text/html')) {
                node.removeAttribute(attribute.name);
            }
        });
    });
    return template.innerHTML;
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



