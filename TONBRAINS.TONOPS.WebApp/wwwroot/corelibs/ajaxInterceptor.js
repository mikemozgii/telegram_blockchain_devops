
function isOpenedOnMobileDevice() {
    const userAgent = navigator.userAgent.toLowerCase();
    return userAgent.indexOf("android") > -1 || userAgent.indexOf("iphone") > -1;
}

const interceptorStatus = {
    enable: false
};

// This file contains global axios interceptor for redirecting, logging and other stuff
axios.interceptors.response.use(
    function (response) {
        //Need force refresh current page if we get 205 code
        if (response.status === 205) location.reload();

        return response;
    },
    function (error) {
        //Case 1: Local redirect to singin page if user don't authorized
        if (error.response.status === 401) {
            if (error.request.responseURL.indexOf(location.origin) === 0) {
                const path = error.request.responseURL.substring(location.origin.length);
                if (path.indexOf(`PageContainer`) > -1 || interceptorStatus.enable) location.href = `/signin`;
            }
        }

        //Case 2: Backend errors fire event
        if (error.response.status === 500) fireEvent(`backendError`, { code: 500 });

        return Promise.reject(error);
    }
);