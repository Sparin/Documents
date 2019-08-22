import React from 'react';
import { Table, Button, Tooltip, Collapse } from 'antd';
import moment from 'moment';
import './Orders.css';
import SearchFilters from '../../components/SearchFilters/SearchFilters';
import { searchOrders } from '../../api/order/methods';
import { SearchResponse, Order, SearchOptions } from '../../api/order/models';
import { Link } from 'react-router-dom';

const { Column } = Table;
const { Panel } = Collapse;



export default class Orders extends React.Component<any, any> {

    constructor(props: any) {
        super(props);
        this.state = {
            orders: [],
            isLoading: true,
            currentPage: 1,
            limit: 50,
            totalPages: 0,
            totalItems: 0,
            searchOptions: {}
        };

        this.applySearchFilters = this.applySearchFilters.bind(this);
    }

    componentDidMount() {
        this.updateOrders(this.state.currentPage, this.state.limit);
    }

    componentDidUpdate(prevProps: any, prevState: any) {
        if (prevState.searchOptions !== this.state.searchOptions)
            this.updateOrders(1, this.state.limit)
    }

    async updateOrders(page: number, limit: number) {
        this.setState({ isLoading: true })
        const results = await searchOrders(page, limit, this.state.searchOptions) as SearchResponse<Order>
        this.setState({
            currentPage: results.currentPage,
            totalPages: results.totalPages,
            totalItems: results.totalItems,
            orders: results.items,
            isLoading: false
        });
    }

    applySearchFilters(options: SearchOptions) {
        let query = {
            eta: { "$gte": options.fromETA, "$lte": options.untilETA },
            amount: { "$gte": options.minimumAmount, "$lte": options.maximumAmount },
            fruit: { "$in": [...options.fruits] },
        }
        this.setState({ searchOptions: query });
    }

    render() {
        return (
            <div className="orders-layout-content">

                <div className="table-control-bar">
                    <Link to="/order/create">
                        <Button type="primary" icon="form">Create</Button>
                    </Link>
                </div>

                <div className="table-filters-bar">
                    <Collapse bordered={false}>
                        <Panel header="Filters" key="1">
                            <SearchFilters onApply={this.applySearchFilters} />
                        </Panel>
                    </Collapse>
                </div>

                <Table dataSource={this.state.orders}
                    loading={this.state.isLoading}
                    style={{ marginTop: "8px" }}
                    rowKey="id"
                    pagination={{
                        onChange: (page, size) =>
                            this.updateOrders(page, size as number)
                        ,
                        showTotal: (total, range) => `Total: ${total}`,
                        current: this.state.currentPage,
                        total: this.state.totalItems,
                        pageSize: this.state.limit
                    }}>
                    <Column title="First Name" dataIndex="firstName" />
                    <Column title="Middle Name" dataIndex="middleName" />
                    <Column title="Last Name" dataIndex="lastName" />
                    <Column title="Address" dataIndex="address" />
                    <Column title="Fruit" dataIndex="fruit" />
                    <Column title="Amount" dataIndex="amount" />
                    <Column title="ETA" dataIndex="eta"
                        render={(text, record) => {
                            const date = new Date(text)
                            return (
                                <Tooltip title={moment(date).format('DD.MM.YYYY HH:mm:ss')}>
                                    <span>{moment(date).fromNow()}</span>
                                </Tooltip>)
                        }} />
                    <Column
                        title="Action"
                        render={(text, record: Order) => (
                            <span>
                                <Link to={`/order/${record._id}`}>View</Link>
                            </span>
                        )}
                    />
                </Table>
            </div >);
    }
}