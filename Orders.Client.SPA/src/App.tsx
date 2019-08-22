import React from 'react';
import './App.css';
import { Route, Switch, Redirect } from 'react-router';
import Layout from './pages/Layout/Layout';
import NotFound from './pages/NotFound/NotFound';
import Orders from './pages/Orders/Orders';
import { BrowserRouter } from 'react-router-dom';
import Order from './components/Order/Order';

const App: React.FC = () => {
  return (
    <div className="App">
      <Layout>
        <BrowserRouter>
          <Switch>
            <Route path="/" exact component={Orders} />
            <Route path="/orders" exact component={Orders} />
            <Route path="/order/:id([a-f\d]{24})?/:action(edit|create)?" exact component={Order} />
            <Route path="/404" component={NotFound} />
            <Redirect from='*' to='/404' />
          </Switch>
        </BrowserRouter>
      </Layout>
    </div>
  );
}

export default App;
