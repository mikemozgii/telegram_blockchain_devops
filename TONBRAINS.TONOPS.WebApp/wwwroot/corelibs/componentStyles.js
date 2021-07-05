const globalTheme = {
    globalGreen: "#00B00D",
    globalRed: "#FF2757",
    globalWhite: "#fff",
    globalBlue: "#ee1c29",
    globalBlack: "#000",
}

const globalComponentStyles = {
    switchBoxes: {
        default: {
            "thumb-background-color": "1B64BF"
        }
    },
    buttons: {
        default: {
            "border": "1px solid",
            "border-color": "#dbdbdb",
            "background-color": "#e0e0e0",
            "color": "#000",
        },
        primaryButton: {
            border: "1px solid",
            "background-color": "#ee1c29",
            color: "#fff",
            height: "31px",
            "font-size": "12px"
        },
        successButton: {
            "border": "1px solid",
            "background-color": "#ee1c29",
            "color": "#fff"
        },
        warningButton: {
            "border": "1px solid",
            "background-color": "rgb(240, 173, 78)",
            "color": "#fff"
        },
        dangerButton: {
            "border": "1px solid",
            "background-color": "#FF2757",
            "color": "#fff"
        },
        passButton: {
            "action-border": "1px solid #00B00D",
            "background-color": "#fff",
            "color": "#00B00D",
            "disable-background-color": "#00B00D",
            "disable-color": "#fff"
        },
        failButton: {
            "action-border": "1px solid #FF2757",
            "background-color": "#fff",
            "color": "#FF2757",
            "disable-background-color": "#FF2757",
            "disable-color": "#fff"
        },
        tasksButton: {
            "border": "1px solid",
            "background-color": "#17a2b8",
            "color": "#fff"
        },
        submitButton: {
            "border": "1px solid",
            "background-color": "#E37641",
            "color": "#fff"
        },
        whiteFillButton: {
            "border": "1px solid",
            "border-color": "#dbdbdb",
            "background-color": "white",
            "color": "#000",
            "hover-border-color": "lighten(#dbdbdb, 5%)"
        },
        coloredButton: {
            "border": "1px solid",
            "border-color": "#dbdbdb",
            "background-color": "#F7F8F9",
            "color": "$global-black",
            "hover-border-color": "lighten(#dbdbdb, 5%)"
        },
        grayEllipseButton: {
            "border-radius": "25px",
            "border-color": "$global-gray",
            "background-color": "#F7F8F9",
            "color": "$global-gray",
            "hover-border-color": "lighten(#dbdbdb, 5%)"
        },
        dangerEllipseButton: {
            "border-radius": "25px",
            "border-color": "$global-red",
            "background-color": "#F7F8F9",
            "color": "$global-red",
            "hover-border-color": "lighten(#dbdbdb, 5%)"
        },
        primaryEllipseButton: {
            border: "1px solid",
            "border-radius": "25px",
            "background-color": "#ee1c29",
            color: "#fff",
            height: "31px",
            "font-size": "12px",
        },
        successEllipseButton: {
            "border-radius": "25px",
            "border-color": "#00B00D",
            "action-border": "1px solid #00B00D",
            "background-color": "#fff",
            "color": "#00B00D",
            "hover-border-color": "lighten(#dbdbdb, 5%)"
        },
        defaultMenuButton: {
            "border": "1px solid",
            "border-color": "#dbdbdb",
            "background-color": "#fff",
            "color": "$global-black",
            "hover-border-color": "lighten(#dbdbdb, 5%)"
        },
        gridMenuButton: {
            "border": "0",
            "border-color": "#dbdbdb",
            "background-color": "#fff",
            "color": "$global-black",
            "hover-border-color": "lighten(#dbdbdb, 5%)",
        },
        horizontalGridMenu: {
            "border": "0",
            "border-color": "#dbdbdb",
            "background-color": "#fff",
            "color": "$global-black",
            "hover-border-color": "lighten(#dbdbdb, 5%)",
        }
    },
    textBoxes: {
        default: {
            "font-size": "1vrem",
            "background-color": "#fff",
            color: "#404040",
            "top-bottom-padding": "5px",
            "right-left-padding": "6px",
            "border-style": "2px solid rgba(0, 0, 0, 0.08)"
        },
        embeddableMultiLine: {
            "font-size": "1.1vrem",
            "background-color": "#fff",
            "color": "#404040",
            "padding": "0px"
        },
        defaultDigit: {
            "font-size": "1vrem",
            "background-color": "#fff",
            "color": "#404040",
            "padding": "0px",//not used
            "top-bottom-padding": "5px",
            "right-left-padding": "6px",
            "border-style": "2px solid rgba(0, 0, 0, 0.08)"
        },
        noPaddingDigit: {
            "font-size": "1vrem",
            "background-color": "#fff",
            "color": "#404040",
            "padding": "0px",//not used
            "top-bottom-padding": "0px",
            "right-left-padding": "0px",
            "border-style": "none"
        },
        defaultMultiLineTextBox: {
            "font-size": "1vrem",
            "background-color": "#fff",
            "color": "#404040"
        }
    },
    hyperLinks: {
        hyperLinkAction: {
            "color": "#ee1c29"
        },
        redHyperLinkAction: {
            "color": "$global-red"
        },
        greenHyperLinkAction: {
            "color": "$global-green"
        },
        blackHyperLinkAction: {
            "color": "$global-black"
        },
        orangeHyperLinkAction: {
            "color": "$global-orange"
        },
        coloredHyperLink: {
            "color": "#ee1c29",
        }
    },
    passwordBoxes: {
        default: {
            "font-size": "1vrem",
            "background-color": "#fff",
            color: "#404040",
            "top-bottom-padding": "5px",
            "right-left-padding": "6px",
            "border-style": "2px solid rgba(0, 0, 0, 0.08)"
        }
    },
    panels: {
        formPanel: {
            "panel-border-width": "1px 1px 3px 1px",
            "border-style": "solid",
            "border-color": "rgba(0,0,0,.125)",
            "footer-padding-left": "0px",
            "footer-padding-right": "0px",
            "footer-padding-top": "0px",
            "footer-padding-bottom": "0px"
        },
        popupPanel: {
            "content-padding-left": "0px",
            "content-padding-right": "0px",
            "content-padding-top": "0px",
            "content-padding-bottom": "0px",
            "border-width": "3px",
            "box-shadow": "0 0 4px 0 rgba(0, 0, 0, .04)"
        },
        contentPanel: {
            contentStyles: {
                "padding-top": "25px",
                "padding-bottom": "15px",
                "background-color": "#fff"
            },
            footerStyles: {
                "display": "none",
                "background-color": "#fff"
            }
        }
    },
    checkboxes: {
        default: {
            "checked-color": "#ee1c29",
            "font-size": "1vrem",
            "header-color": "$global-gray"
        }
    },
    validateLabelBoxes: {
        default: {
            "background-color": "transparent",
            "font-size": "1vrem",
            "font-weight": "bold",
            "not-valid": "#000",
            "color": "#ff6633",
            "container-margin": "0 0 0 0"
        }
    },
    tooltips: {
        default: {
            "transition-delay": "0.7s"
        }
    },
    textBlocks: {
        formFieldLabel: {
            "background-color": "transparent",
            "font-size": "1vrem",
            "font-weight": "bold",
            "color": "$global-black",
            "container-margin": "8px 0 8px 0"
        },
        formFieldLabelRequired: {
            "background-color": "transparent",
            "font-size": "1vrem",
            "font-weight": "bold",
            "color": "$global-red"
        },
        defaultValidateLabelBox: {
            "background-color": "transparent",
            "font-size": "1vrem",
            "font-weight": "bold",
            "not-valid": "$global-black",
            "color": "#ff6633",
            "container-margin": "0 0 0 0"
        }
    },
    modals: {
        defaultFormModal: {
            "panel-width": `0.4`
        }
    },
    modalPages: {
        green: {
            "backgroundTitle": `linear-gradient(269.27deg, #6EAE05 11.55%, #10C659 65.31%)`,
            "iconColorTitle": `#00A341`
        }
    },
    selectBoxes: {
        default: {
            "font-size": `1vrem`,
            "background-color": `#fff`,
            color: "#404040",
            border: "2px solid rgba(0, 0, 0, 0.08)"
        }
    },
    multiSelectBoxes: {
        default: {
            "font-size": `1vrem`,
            "background-color": `#fff`,
            color: `#404040`
        }
    },
    popupPages: {
        green: {
            "background-title": `linear-gradient(269.27deg, #6EAE05 11.55%, #10C659 65.31%)`,
            "icon-background": `#00A341`,
            "icon-color": `white`
        }
    }
}