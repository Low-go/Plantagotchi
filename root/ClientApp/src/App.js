import React, { Component } from 'react';
import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <div className='garden'>
        <h1>Plantagotchi</h1>
      </div>
    );
  }
}
