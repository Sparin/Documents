import React from 'react';
import { Form, DatePicker, Button, Slider, Select } from 'antd';
import { Fruit, SearchOptions } from '../../api/order/models';
import moment from 'moment';

const { RangePicker } = DatePicker;
const { Option } = Select;

export default class SearchFilters extends React.Component<any, any> {

    fruits = Object.keys(Fruit).filter((value, index, self) => self.indexOf(value) === index);

    constructor(props: any) {
        super(props);
        const fruits = Object.keys(Fruit).filter((value, index, self) => self.indexOf(value) === index);
        this.state = {
            fromETA: moment().toDate(),
            untilETA: moment().add(1, 'month').toDate(),
            fruits, minimumAmount: 5, maximumAmount: 10000
        } as SearchOptions;
    }

    onSubmit = (event: any) => {
        if (event)
            event.preventDefault();
        this.props.onApply(this.state);
    }

    render() {
        return (
            <Form layout="vertical" onSubmit={this.onSubmit}>
                <Form.Item label="Estimated time of arrival">
                    <RangePicker
                        defaultValue={[moment(this.state.fromETA), moment(this.state.untilETA)]}
                        showTime
                        format="YYYY-MM-DD HH:mm:ss"
                        onChange={(dates, dateS) => this.setState({ fromETA: dateS[0], untilETA: dateS[1] })} />
                </Form.Item>

                <Form.Item label="Amount">
                    <Slider
                        range
                        value={[this.state.minimumAmount, this.state.maximumAmount]}
                        max={10000}
                        onChange={(values: any) => this.setState({ minimumAmount: values[0], maximumAmount: values[1] })} />
                </Form.Item>

                <Form.Item label="Fruits">
                    <Select
                        mode="multiple"
                        style={{ width: '100%' }}
                        placeholder="Please select fruits for searching"
                        value={this.state.fruits}
                        onChange={(value: string[]) => this.setState({ fruits: value })}
                    >
                        {this.fruits.map(x => (<Option value={x} key={x}>{x}</Option>))}
                    </Select>
                </Form.Item>

                <Form.Item>
                    <Button type="primary" htmlType="submit">Apply</Button>
                </Form.Item>
            </Form>
        )
    }
}