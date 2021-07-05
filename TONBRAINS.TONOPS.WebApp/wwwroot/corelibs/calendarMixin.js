const calendarMixin = {
    locale: {
        data() {
            return {
                i18n: {
                    en: {
                        "weekDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"],
                        "weekDaysShort": ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
                        "weekDaysExtraShort": ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
                        "months": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
                        "years": "Years",
                        "year": "Year",
                        "month": "Month",
                        "week": "Week",
                        "day": "Day",
                        "today": "Today",
                        "noEvent": "No Event",
                        "allDay": "All day",
                        "deleteEvent": "Delete",
                        "createEvent": "Create an event",
                        "dateFormat": "DDDD mmmm d{S}, yyyy",
                        "dateWatermark": "mm/dd/yyyy",
                        "clear": "Clear"
                    },
                    de: {
                        "weekDays": ["Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag", "Sonntag"],
                        "months": ["Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
                        "years": "Jahre",
                        "year": "Jahr",
                        "month": "Monat",
                        "week": "Woche",
                        "day": "Tag",
                        "today": "Heute",
                        "noEvent": "Keine Events",
                        "allDay": "Ganztätig",
                        "deleteEvent": "Löschen",
                        "createEvent": "Event erstellen",
                        "dateFormat": "DDDD d mmmm yyyy",
                        "dateWatermark": "mm/tt/jjjj"
                    },
                    fr: {
                        "weekDays": ["Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche"],
                        "months": ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"],
                        "years": "Années",
                        "year": "Année",
                        "month": "Mois",
                        "week": "Semaine",
                        "day": "Jour",
                        "today": "Aujourd'hui",
                        "noEvent": "Aucun événement",
                        "allDay": "Jour entier",
                        "deleteEvent": "Supprimer",
                        "createEvent": "Créer un événement",
                        "dateFormat": "DDDD d mmmm yyyy",
                        "weekDaysExtraShort": ["Di", "Lu", "Ma", "Me", "Je", "Ve", "Sa"],
                        "dateWatermark": "mm/jj/aaaa",
                        "clear": "Effacer"
                    },
                    es: {
                        "weekDays": ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"],
                        "months": ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                        "years": "Años",
                        "year": "Año",
                        "month": "Mes",
                        "week": "Semana",
                        "day": "Día",
                        "today": "Hoy",
                        "noEvent": "No hay evento",
                        "allDay": "Todo el día",
                        "deleteEvent": "Borrar",
                        "createEvent": "Crear un evento",
                        "dateFormat": "DDDD d mmmm yyyy",
                        "dateWatermark": "mm/dd/aaaa"
                    }
                }
            }
        }
    },
    formatting: {
        methods: {
            addDays(source, days) {
                let date = new Date(source);
                date.setDate(date.getDate() + days);
                return date;
            },
            subtractDays(source, days) {
                let date = new Date(source);
                date.setDate(date.getDate() - days);
                return date;
            },
            getWeek (source) {
                let d = new Date(Date.UTC(source.getFullYear(), source.getMonth(), source.getDate()));
                const dayNum = d.getUTCDay() || 7;
                d.setUTCDate(d.getUTCDate() + 4 - dayNum);
                const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
                return Math.ceil((((d - yearStart) / 86400000) + 1) / 7);
            },
            nth(d) {
                if (d > 3 && d < 21) return 'th'
                switch (d % 10) {
                  case 1: return 'st'
                  case 2: return 'nd'
                  case 3: return 'rd'
                  default: return 'th'
                }
            },
            formatTime(time, format = 'HH:mm') {
                const H = Math.floor(time / 60)
                const h = H % 12 ? H % 12 : 12
                const am = H < 12 ? 'am' : 'pm'
                const m = Math.floor(time % 60)
                const timeObj = {
                    H,
                    h,
                    HH: (H < 10 ? '0' : '') + H,
                    hh: (h < 10 ? '0' : '') + h,
                    am,
                    AM: am.toUpperCase(),
                    m,
                    mm: (m < 10 ? '0' : '') + m
                }
              
                return format.replace(/(\{[a-zA-Z]+\}|[a-zA-Z]+)/g, (m, contents) => timeObj[contents.replace(/\{|\}/g, '')])
            },
            formatDate(date, format = 'yyyy-mm-dd', localizedTexts) {
                const d = date.getDate()
                const m = date.getMonth() + 1
                const dateObj = {
                    D: date.getDay(), // 0 to 6.
                    DD: localizedTexts.weekDays[(date.getDay() - 1 + 7) % 7][0], // M to S.
                    DDD: localizedTexts.weekDays[(date.getDay() - 1 + 7) % 7].substr(0, 3), // Mon to Sun.
                    DDDD: localizedTexts.weekDays[(date.getDay() - 1 + 7) % 7], // Monday to Sunday.
                    d, // 1 to 31.
                    dd: (d < 10 ? '0' : '') + d, // 01 to 31.
                    S: this.nth(d), // st, nd, rd, th.
                    m, // 1 to 12.
                    mm: (m < 10 ? '0' : '') + m, // 01 to 12.
                    mmm: localizedTexts.months[m - 1].substr(0, 3), // Jan to Dec.
                    mmmm: localizedTexts.months[m - 1], // January to December.
                    yyyy: date.getFullYear(), // 2018.
                    yy: date.getFullYear().toString().substr(2, 4) // 18.
                }

                return format.replace(/(\{[a-zA-Z]+\}|[a-zA-Z]+)/g, (m, contents) => {
                    const result = dateObj[contents.replace(/\{|\}/g, '')]
                    return result !== undefined ? result : contents
                })
              }
        }
    },
    legacyService: {
        methods: {
            parseDates(date, timeZone, returnFunction, positiveOffset) {
                const IanaTimeZoneId = timeZone;

                //TODO: you need inject timezone from organization
                //if ($rootScope.organizationSession) IanaTimeZoneId = $rootScope.organizationSession.TimeZone.Name;

                positiveOffset = positiveOffset === true;

                var momentDate = moment.utc(date);
                var offsetMinutes = moment(date).utcOffset() - moment.utc(date).tz(IanaTimeZoneId).utcOffset();

                momentDate.add(positiveOffset ? offsetMinutes : -offsetMinutes, "minutes");

                return returnFunction(momentDate);
            },
            isDataUrl(s) {
                const base64Regex = /^\s*data:([a-z]+\/[a-z]+(;[a-z\-]+\=[a-z\-]+)?)?(;base64)?,[a-z0-9\!\$\&\'\,\(\)\*\+\,\;\=\-\.\_\~\:\@\/\?\%\s]*\s*$/i;
                return !!s.match(base64Regex);
            },
            toOrganizationTimeZone(date, timeZone) {
                return this.parseDates(date, timeZone, function (momentDate) { return momentDate.toDate(); });
            },
            FromOrganizationTimeZone(date, timeZone) {
                return this.parseDates(date, timeZone, function (momentDate) { return momentDate.toDate() }, true);
            },
            extractDateOnly(date) {
                return new Date(date.getFullYear(), date.getMonth(), date.getDate());
            },
            toIsoString(date, timeZone) {
                return this.parseDates(date, timeZone, function (momentDate) { return momentDate.toISOString() }, true);
            },
            datesToOrganizationTimeZoneInModelInner(model) {
                //check if number
                if (!isNaN(model)) return model;
    
                //check if string and base64
                if (typeof model === "string" && isDataUrl(model)) return model;
    
                //check if date
                if (model instanceof Date || (typeof model !== "object" && moment(model).isValid())) return service.toOrganizationTimeZone(model);
    
                //check if not object
                if (typeof model !== "object") return model;
    
                for (var property in model) {
                    if (property !== "RecurrenceException" && model.hasOwnProperty(property)) {
                        if (typeof model[property] === "object" && !(model[property] instanceof Date)) {
                            this.datesToOrganizationTimeZoneInModelInner(model[property]);
                        } else if (isNaN(model[property]) && moment(model[property], moment.ISO_8601, true).isValid()) {
                            model[property] = this.toOrganizationTimeZone(model[property]);
                        }
                    }
                }
    
                return model;
            },
            datesToOrganizationTimeZoneInModel(model) {
                return datesToOrganizationTimeZoneInModelInner(_.cloneDeep(model));
            },
            datesToUtcInModel(model) {
                //check if number
                if (!isNaN(model)) return model;

                //check if string and base64
                if (typeof model === "string" && isDataUrl(model)) return model;

                //check if date
                if (model instanceof Date || (typeof model !== "object" && moment(model).isValid())) return this.toOrganizationTimeZone(model);

                //check if not object
                if (typeof model !== "object") return model;

                for (var property in model) {
                    if (model.hasOwnProperty(property)) {
                        if (typeof model[property] === "object" && !(model[property] instanceof Date)) {
                            this.datesToUtcInModel(model[property]);
                        } else if (model[property] instanceof Date) {
                            model[property] = this.toIsoString(model[property]);
                        }
                    }
                }
    
                return model;
            },
            addMinutes(date, minutes) {
                return new Date(date.getTime() + minutes * 60000);
            },
            toShortDateString (date) {
                var day = date.getDate() >= 10 ? date.getDate() : "0" + date.getDate();
                var numOfMonth = date.getMonth() + 1;
                var month = numOfMonth >= 10 ? numOfMonth : "0" + numOfMonth;
                return day + "." + month + "." + date.getFullYear(); 
            },
            dateIsEqual(date1, date2) {
                return date1.getFullYear() === date2.getFullYear() && date1.getMonth() === date2.getMonth() && date1.getDate() === date2.getDate();
            },
            convertLocalDateToISOString(date) {
                return new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes()))
                    .toISOString();
            },
            convertLocalDateToOrganziationISOString(date) {
                var momentDate = moment.utc(date);
                return momentDate.toISOString();
            }
        }
    }
}