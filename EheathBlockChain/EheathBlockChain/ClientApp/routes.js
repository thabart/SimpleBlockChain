import React, { Component } from "react";
import { Route } from 'react-router-dom';

import Layout from './layout';
import Home from './components/home';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
</Layout>;