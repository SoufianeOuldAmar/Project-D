import axios from 'axios';

const Gateway = axios.create({
    baseURL: process.env.REACT_APP_GATEWAY_API_URL || 'http://localhost:5244', // Gateway URL
});

const Service1 = axios.create({
    baseURL: process.env.REACT_APP_SERVICE1_API_URL || 'http://localhost:5041', // Gateway URL
});


export { Service1, Gateway };
