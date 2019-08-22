import React from 'react';
import { Descriptions, Button, Modal } from 'antd';
import { withRouter, Link } from 'react-router-dom';
import moment from 'moment';

const { confirm } = Modal;

class ViewForm extends React.Component<any, any> {
    showDeleteConfirm = () => {
        const deleteFunc = this.props.onDelete;
        confirm({
            title: 'Are you sure delete this order?',
            okText: 'Yes',
            okType: 'danger',
            cancelText: 'No',
            onOk() {
                deleteFunc();
            },
        });
    }

    render() {
        return (
            <div>
                <Descriptions bordered column={1} style={{ marginBottom: "16px" }}>
                    <Descriptions.Item label="Customer">{this.props.order.lastName} {this.props.order.firstName} {this.props.order.middleName}</Descriptions.Item>
                    <Descriptions.Item label="Fruit">{this.props.order.fruit}</Descriptions.Item>
                    <Descriptions.Item label="Amount">{this.props.order.amount}</Descriptions.Item>
                    <Descriptions.Item label="Callback">
                        {this.props.order.isCallbackRequired ? "Manager will call back to you soon" : "There is no callback for you :P"}
                    </Descriptions.Item>
                    <Descriptions.Item label="Address">{this.props.order.address}</Descriptions.Item>
                    <Descriptions.Item label="Estimated time of arrival">{moment(new Date(this.props.order.eta)).format('DD.MM.YYYY HH:mm:ss')}</Descriptions.Item>
                </Descriptions>
                <div style={{
                    position: "relative",
                    float: "right"
                }}>
                    <Link to={`/order/${this.props.order._id}/edit`} >
                        <Button style={{ marginRight: "8px" }}>Edit</Button>
                    </Link>
                    <Button onClick={this.showDeleteConfirm} type="danger">Delete</Button>
                </div>
            </div>
        );
    }
}

export default withRouter(ViewForm);