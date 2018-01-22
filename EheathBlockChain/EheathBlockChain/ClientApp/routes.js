import React, { Component } from "react";
import { Route, Redirect } from 'react-router-dom';

import Layout from './layout';
import Home from './components/home';
import AddMedicalPrestation from './components/addMedicalPrestation';
import Authenticate from './components/authenticate';
import { SessionService } from './services';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
    <Route exact path='/medicalprestation/add' render={() => SessionService.isExpired() ? (<Redirect to="/" />) : (<AddMedicalPrestation />)} />
    <Route exact path='/authenticate' render={() => SessionService.isExpired() ? (<Authenticate />) : (<Redirect to="/" />)} />
</Layout>;