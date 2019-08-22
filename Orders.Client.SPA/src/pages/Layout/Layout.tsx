import React from 'react';
import * as Ant from 'antd';

const { Header, Footer, Content } = Ant.Layout;

export default class Layout extends React.Component {

    render() {
        return (
            <Ant.Layout style={{ minHeight: "100vh" }}>
                <Header>Header</Header>
                <Content>{this.props.children}</Content>
                <Footer>Footer</Footer>
            </Ant.Layout>);
    }
}