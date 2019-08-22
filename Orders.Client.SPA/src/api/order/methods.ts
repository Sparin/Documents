import { SearchOptions, Order, SearchResponse } from './models';
import makeHttpRequest, { defaultPage, defaultLimit } from '../makeHttpRequest';
import queryString from 'query-string';

export function searchOrders(
    page: number = defaultPage,
    limit: number = defaultLimit,
    options: SearchOptions
) {
    const body = options
    const params = { page, limit };
    const url = `/api/document/order/search?` + queryString.stringify(params);

    return makeHttpRequest<SearchResponse<Order>>(url, 'POST',body);
}

export function getOrder(id: string): Promise<Order> {
    const url = `/api/document/order/${id}`;

    return makeHttpRequest<Order>(url, 'GET');
}

export function createOrder(order: Order): Promise<Order> {
    const body = order
    const url = `/api/document/order`;

    return makeHttpRequest<Order>(url, 'POST', body);
}

export function updateOrder(id: string, order: Order): Promise<Order> {
    const body = order
    const url = `/api/document/order/${id}`;

    return makeHttpRequest<Order>(url, 'PUT', body);
}

export function deleteOrder(id: string) {
    const url = `/api/document/order/${id}`;

    return makeHttpRequest<any>(url, 'DELETE');
}