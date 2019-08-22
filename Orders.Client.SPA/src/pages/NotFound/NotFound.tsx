import React from 'react';
import { Result } from 'antd';

export default class NotFound extends React.Component {
    render() {
        return (<Result
            style={{
                height: "100%"
            }}
            status="404"
            title="Page is not found"/>);
    }
}