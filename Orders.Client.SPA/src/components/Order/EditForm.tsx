import React from 'react';
import { Form, Input, Radio, Button, InputNumber, DatePicker, Checkbox } from 'antd';
import { withRouter } from 'react-router-dom';
import { Fruit } from '../../api/order/models';
import moment from 'moment';

class EditForm extends React.Component<any, any> {

    fruits = Object.keys(Fruit).filter((value, index, self) => self.indexOf(value) === index);

    constructor(props: any) {
        super(props);

        this.state = { ...this.props.order };
    }

    onSubmit = (event: any) => {
        if (event)
            event.preventDefault();
        this.props.form.validateFields((err: any) => {
            if (!err) {
                this.props.onSubmit(this.state._id, { ...this.props.form.getFieldsValue() });
            }
        })
    };

    render() {
        const { getFieldDecorator } = this.props.form;

        return (
            <Form layout="vertical" onSubmit={this.onSubmit}>
                <Form.Item label="Last name">
                    {getFieldDecorator('lastName', {
                        initialValue: this.state.lastName,
                        rules: [{ required: true, message: 'Please input your last name!' }],
                    })(<Input onChange={e => this.props.form.setFieldsValue({ lastName: e.target.value })} />)}
                </Form.Item>

                <Form.Item label="First name">
                    {getFieldDecorator('firstName', {
                        initialValue: this.state.firstName,
                        rules: [{ required: true, message: 'Please input your first name!' }],
                    })(<Input onChange={e => this.props.form.setFieldsValue({ firstName: e.target.value })} />)}
                </Form.Item>

                <Form.Item label="Middle name">
                    {getFieldDecorator('middleName', {
                        initialValue: this.state.middleName,
                        rules: [],
                    })(<Input onChange={e => this.props.form.setFieldsValue({ middleName: e.target.value })} />)}
                </Form.Item>

                <Form.Item label="Fruit" >
                    {getFieldDecorator('fruit', {
                        initialValue: this.state.fruit,
                        rules: [{ required: true, message: 'Please select one of the freshest fruits' }],
                    })(
                        <Radio.Group onChange={e => this.props.form.setFieldsValue({ fruit: e.target.value })}>
                            {this.fruits.map(fruit => <Radio value={fruit} key={fruit}>{fruit}</Radio>)}
                        </Radio.Group>)}
                </Form.Item>

                <Form.Item label="Amount" >
                    {getFieldDecorator('amount', {
                        initialValue: this.state.amount,
                        rules: [{
                            validator: (rule: any, value: number, callback: any) => value <= 0 ? callback('The amount should be a positive number') : callback(),
                            message: 'The amount should be a positive number'
                        }],
                    })(<InputNumber style={{ width: "100%" }} min={0} onChange={value => this.props.form.setFieldsValue({ amount: value })} />)}
                </Form.Item>

                <Form.Item label="Address">
                    {getFieldDecorator('address', {
                        initialValue: this.state.address,
                        rules: [{ required: true, message: 'Please input your address!' }],
                    })(<Input onChange={e => this.props.form.setFieldsValue({ address: e.target.value })} />)}
                </Form.Item>

                <Form.Item label="Estimated time of arrival">
                    {getFieldDecorator('eta', {
                        initialValue: moment(this.state.eta),
                        rules: [{ required: true, message: 'Please select estimated time of arrival' }],
                    })(<DatePicker
                        style={{ width: "100%" }}
                        showTime
                        format="YYYY-MM-DD HH:mm:ss"
                        onChange={(value: any, dateString) => this.props.form.setFieldsValue({ eta: moment(value).format() })} />)}
                </Form.Item>

                <Form.Item>
                    {getFieldDecorator('isCallbackRequired', {
                        initialValue: this.state.isCallbackRequired,
                        valuePropName: 'checked',
                    })(<Checkbox onChange={e => this.props.form.setFieldsValue({ isCallbackRequired: e.target.checked })}>Call me back for details</Checkbox>)}
                </Form.Item>

                <Form.Item >
                    <Button style={{ width: "100%" }} type="primary" onClick={this.onSubmit}>
                        Submit
                    </Button>
                </Form.Item>
            </Form>
        )
    }
}

export default withRouter(EditForm);