import React from 'react';
import { Spin, PageHeader, Form } from 'antd';
import ViewForm from './ViewForm';
import EditForm from './EditForm';
import { getOrder, updateOrder, createOrder, deleteOrder } from '../../api/order/methods';
import { withRouter } from 'react-router';
import * as Models from '../../api/order/models';

class Order extends React.Component<any, any> {

    constructor(props: any) {
        super(props);

        let action = "view";
        let id = this.props.match.params.id;
        if (this.props.match.params.action)
            action = this.props.match.params.action;
        this.state = { id, action, order: {}, isLoading: true, errors: undefined };
        this.onSubmit = this.onSubmit.bind(this);
        this.onDelete = this.onDelete.bind(this);
    }

    componentDidMount() {
        if (this.props.match.params.action === 'create')
            this.setState({ isLoading: false });
        else
            getOrder(this.state.id)
                .then(order => {
                    this.setState({ order, isLoading: false });
                })
                .catch(err => {
                    this.props.history.push('/404')
                });
    }

    onSubmit(id:string, order: Models.Order) {
        this.setState({ isLoading: true });
        let request;
        switch (this.props.match.params.action) {
            case "create": request = createOrder(order); break;
            case "edit": request = updateOrder(id, order); break;
            default: throw Error("Unpredictable action on order submit");
        }

        request.then(order => {
            this.setState({ order, isLoading: false });
            this.props.history.push(`/order/${order._id}`)
        }).catch(err => {

        });
    }

    async onDelete() {
        this.setState({ isLoading: true });
        await deleteOrder(this.props.match.params.id);
        this.props.history.push('/');
    }

    render() {
        if (this.state.isLoading)
            return (<Spin tip="Loading..." style={{ width: "100%", margin: "16px 0" }} />);
        let form;
        const WrappedEditForm = Form.create<any>()(EditForm);
        switch (this.props.match.params.action) {
            case "create":
            case "edit": form = <WrappedEditForm order={this.state.order} action={this.props.match.params.action} onSubmit={this.onSubmit} errors={this.state.errors} />; break;
            default: form = (<ViewForm order={this.state.order} onDelete={this.onDelete} />); break;
        }

        return (<div>
            <PageHeader onBack={() => this.props.history.goBack()} title={this.props.match.params.action !== 'create' ? `Order #${this.state.order._id}` : 'Create new order'} />
            <div style={{ margin: "16px", display: "flex", justifyContent: "center" }}>
                {form}
            </div>
        </div>)
    }
}

export default withRouter(Order);