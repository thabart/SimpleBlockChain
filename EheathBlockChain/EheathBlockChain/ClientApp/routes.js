import React, { Component } from "react";
import { Route } from 'react-router-dom';

import Layout from './layout';
import Home from './components/home';
import AddMedicalPrestation from './components/addMedicalPrestation';
import Authenticate from './components/authenticate';

export const routes = <Layout>
    <Route exact path='/' component={Home} />
    <Route exact path='/medicalprestation/add' component={AddMedicalPrestation} />
    <Route exact path='/authenticate' component={Authenticate} />
</Layout>;